using System.Collections;
using System.Collections.Generic;
using Gameplay.CharacterComponents;
using Gameplay.CharacterComponents.Cpu;
using Gameplay.Managers;
using TMPro;
using UnityEngine;

public class TimedBallTutorial : MonoBehaviour
{
    const float SuccessfulKickSlowMotionDuration = 1.5f;
    const float KickSpeedRecoveryDuration = 0.35f;
    const float SuccessfulKickFlightDuration = 2.5f;

    public enum PracticeType { Kick, Block }

    [SerializeField] TextMeshProUGUI _promptText;
    [SerializeField, Min(0.1f)] float _flightDuration = 1.25f;
    [SerializeField, Min(0.1f)] float _attemptDuration = 4f;
    [SerializeField, Min(0f)] float _readyDelay = 0.5f;
    [SerializeField, Min(0f)] float _retryDelay = 1f;
    [SerializeField, Min(0f)] float _blockHeightAboveHead = 0.6f;

    class ActionState
    {
        public PlayerActions Actions;
        public bool CanKick;
        public bool ForcedToHoldKick;
        public bool DisableInput;
        public Cpu Cpu;
        public bool CpuEnabled;
    }

    readonly List<ActionState> _actionStates = new();
    BallScript _ball;
    BallManager _ballManager;
    MatchManager _matchManager;
    PlayerActions _player;
    PlayerActions _grandpaActions;
    GrandpaStage0 _grandpa;
    MoveToPointCurved _kickFlightMover;
    PracticeType _practiceType;
    bool _running;
    bool _attemptActive;
    bool _previousResetSuppression;
    bool _matchWasEnabled;

    public bool Succeeded { get; private set; }

    public IEnumerator Run(PracticeType practiceType, TutorialFade prompt, MoveToPointCurved ballMove)
    {
        Succeeded = false;
        PlayersManager players = PlayersManager.Instance;
        _ballManager = BallManager.Instance;
        _ball = _ballManager != null ? _ballManager.Ball : null;
        _grandpa = GrandpaStage0.Instance;
        _player = players != null && players.Players.Count > 0
            ? players.Players[0].GetComponent<PlayerActions>()
            : null;
        _grandpaActions = _grandpa != null ? _grandpa.GetComponent<PlayerActions>() : null;
        if (_running || _ball == null || _player == null || _grandpaActions == null)
        {
            Debug.LogError("Timing practice needs the tutorial player, ball, and Grandpa.", this);
            yield break;
        }

        Collider2D targetCollider = practiceType == PracticeType.Kick
            ? _player.KickingContactCollider
            : _player.HeadContactCollider;
        if (targetCollider == null)
        {
            Debug.LogError("Timing practice needs the player's kicking leg or head collider.", this);
            yield break;
        }

        _running = true;
        _practiceType = practiceType;
        _previousResetSuppression = _ballManager.SuppressAutomaticReset;
        _ballManager.SuppressAutomaticReset = true;
        _matchManager = MatchManager.Instance;
        _matchWasEnabled = _matchManager != null && _matchManager.enabled;
        if (_matchManager != null)
            _matchManager.enabled = false;

        // Grandpa feeds deliberately; goal/out notifications must not interrupt a practice attempt.
        foreach (PlayerActions actions in players.GetPlayerActions())
        {
            Cpu cpu = actions.GetComponent<Cpu>();
            _actionStates.Add(new ActionState
            {
                Actions = actions,
                CanKick = actions.CanKick,
                ForcedToHoldKick = actions.ForcedToHoldKick,
                DisableInput = actions.DisableInput,
                Cpu = cpu,
                CpuEnabled = cpu != null && cpu.enabled
            });
            if (cpu != null)
            {
                cpu.StopAllCoroutines();
                cpu.enabled = false;
            }
            actions.ResetActionState();
            actions.CanKick = false;
            actions.ForcedToHoldKick = false;
            actions.DisableInput = actions != _player;
        }

        _kickFlightMover = ballMove;
        ballMove?.StopCurveMove();
        _ball.ContactedPlayer += OnBallContactedPlayer;
        TimeScaleManager.SetGameplayTimeScale();
        SetPrompt(false);
        prompt?.FadeIn();

        try
        {
            while (!Succeeded && _running && _ball != null && _player != null && _grandpa != null)
            {
                _attemptActive = false;
                _player.CanKick = false;
                _ball.Rigidbody.bodyType = RigidbodyType2D.Kinematic;
                _ball.ResetWithoutBlink();
                players.ResetPlayers();
                _ball.Rigidbody.position = GetFeedPosition();
                _ball.Renderer.enabled = true;
                _ball.Collider.enabled = true;
                yield return new WaitForSeconds(_readyDelay);

                if (!_running || _ball == null || _player == null || _grandpa == null)
                    yield break;

                FeedBall(targetCollider);
                _player.CanKick = true;
                _attemptActive = true;
                float elapsed = 0f;
                float attemptDuration = Mathf.Max(_attemptDuration, _flightDuration + 0.5f);
                bool grandpaLegReturned = false;
                while (!Succeeded && _running && _ball != null && elapsed < attemptDuration)
                {
                    elapsed += Time.deltaTime;
                    if (!grandpaLegReturned && elapsed >= 0.2f)
                    {
                        _grandpaActions.ReturnLeftLegToOriginalPosition();
                        grandpaLegReturned = true;
                    }
                    yield return null;
                }

                _attemptActive = false;
                if (!Succeeded && _running)
                {
                    SetPrompt(true);
                    yield return new WaitForSeconds(_retryDelay);
                }
            }

            if (Succeeded)
            {
                if (_promptText != null)
                    _promptText.text = "Good timing!";

                if (_practiceType == PracticeType.Kick)
                {
                    yield return WaitForSuccessfulKickFlight(_ball);
                }
                else
                {
                    _player.OnActionCancelled();
                    _player.CanKick = false;
                    yield return new WaitForSeconds(0.5f);
                }
                prompt?.FadeOut();
            }
        }
        finally
        {
            RestorePracticeState();
        }
    }

    Vector2 GetFeedPosition()
    {
        float direction = Mathf.Sign(_player.transform.position.x - _grandpa.transform.position.x);
        return (Vector2)_grandpa.transform.position + new Vector2(direction * 1.25f, 1.5f);
    }

    void FeedBall(Collider2D targetCollider)
    {
        Vector2 start = GetFeedPosition();
        Vector2 target = targetCollider.bounds.center;
        if (_practiceType == PracticeType.Block)
            target.y += _blockHeightAboveHead;

        float flightTime = Mathf.Max(0.1f, _flightDuration);
        float damping = _ball.Rigidbody.linearDamping;
        float travelFactor = damping > Mathf.Epsilon
            ? (1f - Mathf.Exp(-damping * flightTime)) / damping
            : flightTime;
        float gravityFactor = damping > Mathf.Epsilon
            ? (flightTime - travelFactor) / damping
            : 0.5f * flightTime * flightTime;
        Vector2 gravity = Physics2D.gravity * _ball.Rigidbody.gravityScale;

        _grandpaActions.CanKick = true;
        _grandpaActions.DisableInput = false;
        _grandpa.DoScriptedKick();
        _grandpaActions.CanKick = false;
        _grandpaActions.DisableInput = true;

        _ball.Rigidbody.position = start;
        _ball.Rigidbody.simulated = true;
        _ball.Rigidbody.bodyType = RigidbodyType2D.Dynamic;
        _ball.Rigidbody.linearVelocity = (target - start - gravity * gravityFactor) / travelFactor;
        _ball.Rigidbody.angularVelocity = 0f;
        // After launch, normal physics and the player's usual action determine the result.
    }

    void OnBallContactedPlayer(PlayerActions actions, Collider2D bodyCollider)
    {
        if (Succeeded || !_attemptActive || actions != _player || !actions.IsWithinActionContactWindow)
            return;

        bool correctContact = _practiceType == PracticeType.Kick
            ? actions.IsKickingContact(bodyCollider)
            : bodyCollider == actions.HeadContactCollider;
        if (correctContact)
        {
            Succeeded = true;
            if (_practiceType == PracticeType.Kick)
                BeginSuccessfulKick(actions, _ball, _kickFlightMover);
        }
    }

    public static void BeginSuccessfulKick(PlayerActions actions, BallScript ball, MoveToPointCurved flightMover)
    {
        actions.CompleteTutorialKick();
        TimeScaleManager.SlowMotion();
        GrandpaStage0 grandpa = GrandpaStage0.Instance;
        if (ball != null && flightMover != null && grandpa != null)
        {
            Vector2 target = (Vector2)grandpa.transform.position + Vector2.up * 2.2f;
            flightMover.StartKickFlight(ball, target);
        }
    }

    public static IEnumerator WaitForSuccessfulKickFlight(BallScript ball)
    {
        float lastAppliedScale = TimeScaleManager.SlowMotionScale;
        bool restoreSpeedOnExit = true;
        try
        {
            float slowMotionElapsed = 0f;
            while (ball != null && slowMotionElapsed < SuccessfulKickSlowMotionDuration)
            {
                if (Time.timeScale > 0f)
                {
                    TimeScaleManager.SlowMotion();
                    lastAppliedScale = Time.timeScale;
                    slowMotionElapsed += Time.unscaledDeltaTime;
                }
                yield return null;
            }

            if (ball == null)
                yield break;

            float recoveryElapsed = 0f;
            while (ball != null && recoveryElapsed < KickSpeedRecoveryDuration)
            {
                if (Time.timeScale > 0f)
                {
                    recoveryElapsed += Time.unscaledDeltaTime;
                    lastAppliedScale = Mathf.Lerp(TimeScaleManager.SlowMotionScale,
                        TimeScaleManager.GameplayScale,
                        Mathf.Clamp01(recoveryElapsed / KickSpeedRecoveryDuration));
                    Time.timeScale = lastAppliedScale;
                }
                yield return null;
            }

            if (ball == null)
                yield break;

            TimeScaleManager.SetGameplayTimeScale();
            restoreSpeedOnExit = false;

            // Slow motion is extra breathing room, followed by the full normal-speed flight window.
            float flightElapsed = 0f;
            while (ball != null && flightElapsed < SuccessfulKickFlightDuration)
            {
                if (Time.timeScale > 0f)
                    flightElapsed += Time.unscaledDeltaTime;
                yield return null;
            }
        }
        finally
        {
            if (restoreSpeedOnExit && Time.timeScale > 0f &&
                Mathf.Approximately(Time.timeScale, lastAppliedScale))
                TimeScaleManager.SetGameplayTimeScale();
        }
    }

    void SetPrompt(bool retry)
    {
        if (_promptText == null)
            return;
        string action = _practiceType == PracticeType.Kick ? "kick" : "block";
        _promptText.text = retry
            ? $"Try again: time your {action} with Z"
            : $"Time your {action} with Z";
    }

    void OnDisable() => RestorePracticeState();

    void RestorePracticeState()
    {
        if (!_running)
            return;

        _running = false;
        _attemptActive = false;
        _kickFlightMover?.StopCurveMove();
        _kickFlightMover = null;
        if (_ball != null)
            _ball.ContactedPlayer -= OnBallContactedPlayer;
        if (_ballManager != null)
            _ballManager.SuppressAutomaticReset = _previousResetSuppression;
        if (_matchManager != null)
            _matchManager.enabled = _matchWasEnabled;
        foreach (ActionState state in _actionStates)
        {
            if (state.Actions != null)
            {
                state.Actions.ResetActionState();
                state.Actions.CanKick = state.CanKick;
                state.Actions.ForcedToHoldKick = state.ForcedToHoldKick;
                state.Actions.DisableInput = state.DisableInput;
            }
            if (state.Cpu != null)
                state.Cpu.enabled = state.CpuEnabled;
        }
        _actionStates.Clear();
    }
}
