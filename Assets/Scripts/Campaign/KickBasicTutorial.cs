using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using Gameplay.Managers;
using Gameplay.CharacterComponents;

public class KickBasicTutorial : MonoBehaviour
{
    const float KickContactTimeout = 2f;

    [SerializeField] private float InitialDelay;
    [SerializeField] private TutorialFade TutorialFade;
    [SerializeField] private TimedBallTutorial timingPractice;

    public UnityEvent onTutorialDone;

    List<PlayerActions> playerActions;
    Coroutine _tutorialCoroutine;
    BallScript _guidedBall;
    PlayerActions _mainPlayer;
    BallManager _ballManager;
    MatchManager _matchManager;
    bool _previousResetSuppression;
    bool _matchWasEnabled;
    bool _ownsTutorialState;
    bool _waitingForKickContact;
    bool _kickConnected;
    bool _protectDrop;

    class DropCollisionState
    {
        public Collider2D Collider;
        public bool WasIgnored;
        public bool Restored;
    }

    readonly List<DropCollisionState> _dropCollisions = new();

    public void StartTutorial()
    {
        if (_tutorialCoroutine == null)
            _tutorialCoroutine = StartCoroutine(tutorialRoutine());
    }

    KeyCode kick = KeyCode.Z;
    public float stopDistance;
    public Transform ballDropTarget;
    public MoveToPointCurved ballMoveScript;
    IEnumerator tutorialRoutine()
    {
        yield return new WaitForSeconds(0.1f);
        PlayersManager players = PlayersManager.Instance;
        _ballManager = BallManager.Instance;
        BallScript ball = _ballManager != null ? _ballManager.Ball : null;
        _mainPlayer = players != null && players.Players.Count > 0
            ? players.Players[0].GetComponent<PlayerActions>()
            : null;
        if (ball == null || _mainPlayer == null || ballDropTarget == null)
        {
            Debug.LogError("The kick lesson needs its player, ball, and drop target.", this);
            _tutorialCoroutine = null;
            yield break;
        }

        playerActions = players.GetPlayerActions();
        _guidedBall = ball;
        _guidedBall.ContactedPlayer += OnBallContactedPlayer;
        _previousResetSuppression = _ballManager.SuppressAutomaticReset;
        _ballManager.SuppressAutomaticReset = true;
        _matchManager = MatchManager.Instance;
        _matchWasEnabled = _matchManager != null && _matchManager.enabled;
        if (_matchManager != null)
            _matchManager.enabled = false;
        _ownsTutorialState = true;
        _kickConnected = false;

        try
        {
            ToggleKickAllowed(false);
            ToggleForceKickToHold(true);
            _mainPlayer.ResetActionState();
            IgnoreHeadAndArmsDuringDrop();
            if (InitialDelay > 0)
                yield return new WaitForSeconds(InitialDelay);

            while (!_kickConnected && ball != null)
            {
                bool kickRequested = false;
                bool prompted = false;
                float startDistance = Mathf.Max(0.01f, distance());
                while (!_kickConnected && ball != null && Time.timeScale > 0f)
                {
                    if (Time.timeScale < 0.13f && !prompted)
                    {
                        _waitingForKickContact = true;
                        _mainPlayer.CanKick = true;
                        TutorialFade?.FadeIn();
                        prompted = true;
                    }

                    if (prompted && Input.GetKey(kick))
                    {
                        kickRequested = true;
                        break;
                    }
                    Time.timeScale = Mathf.Clamp01(distance() / startDistance);
                    yield return null;
                }

                if (ball == null)
                    yield break;

                if (!_kickConnected && !kickRequested)
                {
                    _waitingForKickContact = true;
                    _mainPlayer.CanKick = true;
                    TutorialFade?.FadeIn();
                    yield return new WaitUntil(() => ball == null || _kickConnected || Input.GetKey(kick));
                }

                if (ball == null)
                    yield break;

                // Input can arrive in the same frame that the paused lesson enables the control.
                if (!_kickConnected && !_mainPlayer.IsWithinActionContactWindow)
                    _mainPlayer.OnActionPerformed();
                if (!_kickConnected)
                    TimeScaleManager.SetGameplayTimeScale();

                float contactWait = 0f;
                while (!_kickConnected && ball != null && contactWait < KickContactTimeout)
                {
                    contactWait += Time.deltaTime;
                    yield return null;
                }

                if (!_kickConnected && ball != null)
                {
                    // An empty swing repeats this lesson instead of advancing or waiting forever.
                    _waitingForKickContact = false;
                    ToggleKickAllowed(false);
                    TutorialFade?.FadeOut();
                    RestoreDropCollisions();
                    players.ResetMainPlayer();
                    _ballManager.ResetBall();
                    IgnoreHeadAndArmsDuringDrop();
                    yield return new WaitForSeconds(1.1f);
                }
            }

            if (ball == null)
                yield break;

            _protectDrop = false;
            ToggleForceKickToHold(false);
            ToggleKickAllowed(false);
            TutorialFade?.FadeOut();

            yield return TimedBallTutorial.WaitForSuccessfulKickFlight(ball);
            RestoreDropCollisions();

            if (ball == null)
                yield break;

            if (timingPractice != null)
            {
                yield return timingPractice.Run(TimedBallTutorial.PracticeType.Kick, TutorialFade, ballMoveScript);
                if (!timingPractice.Succeeded)
                    yield break;
            }

        }
        finally
        {
            ReleaseTutorialState();
            _tutorialCoroutine = null;
        }

        onTutorialDone?.Invoke();
        Stage0Scene2Tutorials.Instance?.NextTutorial();

        float distance() => Vector2.Distance(ball.transform.position, ballDropTarget.position) - stopDistance;
    }

    void OnBallContactedPlayer(PlayerActions actions, Collider2D bodyCollider)
    {
        if (!_waitingForKickContact || actions != _mainPlayer ||
            !actions.IsWithinActionContactWindow || !actions.IsKickingContact(bodyCollider))
            return;

        _kickConnected = true;
        _waitingForKickContact = false;
        TimedBallTutorial.BeginSuccessfulKick(actions, _guidedBall, ballMoveScript);
    }

    void IgnoreHeadAndArmsDuringDrop()
    {
        _protectDrop = true;
        // Let the first kick lesson's drop pass the head and arms to reach the kicking leg.
        foreach (Collider2D bodyCollider in _mainPlayer.GetComponentsInChildren<Collider2D>())
        {
            string bodyPart = bodyCollider.gameObject.name;
            if (bodyCollider.isTrigger ||
                (bodyPart != "Head" && bodyPart != "LeftArm" && bodyPart != "RightArm"))
                continue;

            _dropCollisions.Add(new DropCollisionState
            {
                Collider = bodyCollider,
                WasIgnored = Physics2D.GetIgnoreCollision(_guidedBall.Collider, bodyCollider)
            });
        }
        UpdateDropCollisions();
    }

    void FixedUpdate() => UpdateDropCollisions();

    void UpdateDropCollisions()
    {
        if (_guidedBall == null || !_guidedBall.Collider.enabled)
            return;

        foreach (DropCollisionState state in _dropCollisions)
        {
            if (state.Restored || state.Collider == null)
                continue;

            Bounds ballBounds = _guidedBall.Collider.bounds;
            Bounds bodyBounds = state.Collider.bounds;
            bool clearOfBodyPart = _protectDrop
                ? ballBounds.max.y < bodyBounds.min.y
                : !ballBounds.Intersects(bodyBounds);
            Physics2D.IgnoreCollision(_guidedBall.Collider, state.Collider,
                !clearOfBodyPart || state.WasIgnored);
            state.Restored = clearOfBodyPart;
        }
    }

    void RestoreDropCollisions()
    {
        if (_guidedBall != null)
        {
            foreach (DropCollisionState state in _dropCollisions)
            {
                if (state.Collider != null)
                    Physics2D.IgnoreCollision(_guidedBall.Collider, state.Collider, state.WasIgnored);
            }
        }
        _dropCollisions.Clear();
    }

    void OnDisable()
    {
        if (_tutorialCoroutine != null)
            StopCoroutine(_tutorialCoroutine);
        _tutorialCoroutine = null;
        ReleaseTutorialState();
    }

    void ReleaseTutorialState()
    {
        ballMoveScript?.StopCurveMove();
        RestoreDropCollisions();
        if (_guidedBall != null)
            _guidedBall.ContactedPlayer -= OnBallContactedPlayer;
        _guidedBall = null;
        _waitingForKickContact = false;
        if (!_ownsTutorialState)
            return;

        _ownsTutorialState = false;
        if (_ballManager != null)
            _ballManager.SuppressAutomaticReset = _previousResetSuppression;
        if (_matchManager != null)
            _matchManager.enabled = _matchWasEnabled;
    }

    void ToggleKickAllowed(bool allowed)
    {
        if (playerActions != null)
        {
            foreach (PlayerActions playerAction in playerActions)
            {
                if (playerAction != null)
                    playerAction.CanKick = allowed;
            }
        }
    }

    void ToggleForceKickToHold(bool force)
    {
        if (playerActions != null)
        {
            foreach (PlayerActions playerAction in playerActions)
            {
                if (playerAction != null)
                    playerAction.ForcedToHoldKick = force;
            }
        }
    }

}
