using System.Collections;
using System.Collections.Generic;
using CommonDataTypes;
using EventBusSystem;
using Gameplay.CharacterComponents;
using Gameplay.CharacterComponents.Cpu;
using Gameplay.Managers;
using UnityEngine;
using UnityEngine.Events;

public class GoalBasicTutorial : MonoBehaviour
{
    [SerializeField] TutorialFade _tutorialFade;
    [SerializeField] UiManager _uiManager;
    [SerializeField] Vector2 _neutralBallPosition = new Vector2(0f, -1.5f);
    [SerializeField, Min(0f)] float _introDelay = 1.5f;
    [SerializeField, Min(0f)] float _retryDelay = 1f;
    [SerializeField, Min(0f)] float _successDialogueDuration = 1.5f;
    [SerializeField, Min(0f)] float _completionDelay = 2f;

    [Header("Grandpa Practice AI")]
    [SerializeField] CpuDifficultyPreset _difficultyPreset;
    [SerializeField] DifficultyLevel _difficulty = DifficultyLevel.Easy1;
    [SerializeField] CpuDifficultyPreset.FloatRange _timeBetweenActions =
        new CpuDifficultyPreset.FloatRange { Min = 0.9f, Max = 1.8f };
    [Tooltip("Horizontal area Grandpa can move toward when defending the goal.")]
    [SerializeField] Vector2 _grandpaMovementLimits = new Vector2(1.5f, 8.5f);
    [Tooltip("How far ahead Grandpa starts moving his body toward an approaching ball.")]
    [SerializeField, Min(0f)] float _grandpaApproachDistance = 4.5f;
    [SerializeField, Min(0.1f)] float _returnArcHeight = 0.65f;
    [SerializeField, Min(0.1f)] float _maximumReturnSpeed = 8f;
    [SerializeField, Min(0.1f)] float _receiveDistance = 1f;

    public UnityEvent onTutorialDone;

    class ActionState
    {
        public PlayerActions Actions;
        public bool CanKick;
        public bool DisableInput;
        public bool ForcedToHoldKick;
        public bool LockMovement;
    }

    readonly List<ActionState> _actionStates = new();
    PlayersManager _players;
    MatchManager _match;
    BallManager _ballManager;
    BallScript _ball;
    PlayerActions _player;
    PlayerActions _grandpaActions;
    Cpu _grandpaCpu;
    Coroutine _routine;
    bool _active;
    bool _roundActive;
    bool _succeeded;
    bool _playerHasTouchedBall;
    bool _returningToPlayer;
    bool _previousMatchSuppression;
    bool _previousResetSuppression;
    bool _previousCpuEnabled;
    Vector2? _previousResetPosition;

    public void StartTutorial()
    {
        if (_active)
            return;

        _players = PlayersManager.Instance;
        _match = MatchManager.Instance;
        _ballManager = BallManager.Instance;
        _ball = _ballManager != null ? _ballManager.Ball : null;
        _player = _players != null && _players.Players.Count > 0
            ? _players.Players[0].GetComponent<PlayerActions>()
            : null;
        GrandpaStage0 grandpa = GrandpaStage0.Instance;
        _grandpaActions = grandpa != null ? grandpa.GetComponent<PlayerActions>() : null;
        _grandpaCpu = grandpa != null ? grandpa.GetComponent<Cpu>() : null;
        if (_match == null || _ball == null || _player == null ||
            _grandpaActions == null || _grandpaCpu == null || _difficultyPreset == null)
        {
            Debug.LogError("The goal lesson needs its player, Grandpa CPU, ball, match, and difficulty preset.", this);
            return;
        }

        _active = true;
        _succeeded = false;
        _previousMatchSuppression = _match.SuppressGoalAndOutEvents;
        _previousResetSuppression = _ballManager.SuppressAutomaticReset;
        _previousResetPosition = _ballManager.ResetPositionOverride;
        _previousCpuEnabled = _grandpaCpu.enabled;
        // Keep the match enabled so campaign execution does not advance or rebuild it.
        _match.SuppressGoalAndOutEvents = true;
        _ballManager.ResetPositionOverride = _neutralBallPosition;
        foreach (PlayerActions actions in _players.GetPlayerActions())
        {
            _actionStates.Add(new ActionState
            {
                Actions = actions,
                CanKick = actions.CanKick,
                DisableInput = actions.DisableInput,
                ForcedToHoldKick = actions.ForcedToHoldKick,
                LockMovement = actions.LockMovementToFacingDirection
            });
            actions.ForcedToHoldKick = false;
        }
        // Free practice needs the normal ability to move back toward a returned ball.
        _player.LockMovementToFacingDirection = false;

        CpuDifficultyPreset.DifficultySettings source = _difficultyPreset.GetSettingsForDifficulty(_difficulty);
        _grandpaCpu.BeginTutorialPractice(new CpuDifficultyPreset.DifficultySettings
        {
            ReactionTime = source.ReactionTime,
            ProximityPoints = source.ProximityPoints,
            TimeBetweenKicks = _timeBetweenActions
        }, _grandpaMovementLimits, _grandpaApproachDistance);
        EventBus<GoalEvent>.OnEvent += OnGoal;
        EventBus<OutEvent>.OnEvent += OnOut;
        _ball.ContactedPlayer += OnBallContactedPlayer;
        TimeScaleManager.SetGameplayTimeScale();
        _uiManager?.ChangeScore(0, 0);
        _routine = StartCoroutine(TutorialRoutine());
    }

    IEnumerator TutorialRoutine()
    {
        bool retry = false;
        while (_active && !_succeeded)
        {
            _roundActive = false;
            _returningToPlayer = false;
            _playerHasTouchedBall = false;
            SetActionsEnabled(false);
            _ballManager.SuppressAutomaticReset = true;
            GoalsManager.Instance?.SetCollidersEnabled(false);
            _players.ResetPlayers();
            _ballManager.ResetBall();
            Stage0Scene2Tutorials dialogue = Stage0Scene2Tutorials.Instance;
            if (retry)
                dialogue?.ShowGoalRetry(_tutorialFade);
            else
                dialogue?.ShowGoalInstruction(_tutorialFade);

            yield return WaitForDialogue(_introDelay);
            while (_active && _ball.Rigidbody.bodyType != RigidbodyType2D.Dynamic)
                yield return null;
            if (!_active)
                yield break;

            GoalsManager.Instance?.SetCollidersEnabled(true);
            _ballManager.SuppressAutomaticReset = false;
            _roundActive = true;
            SetActionsEnabled(true);
            while (_active && _roundActive)
                yield return null;
            if (!_succeeded)
            {
                yield return WaitForDialogue(_retryDelay);
                retry = true;
            }
        }

        if (!_active)
            yield break;

        SetActionsEnabled(false);
        _ballManager.SuppressAutomaticReset = true;
        _ball.StopAllCoroutines();
        _ball.ResetWithoutBlink();
        _ball.Rigidbody.bodyType = RigidbodyType2D.Kinematic;
        _uiManager?.ChangeScore(1, 0);
        Stage0Scene2Tutorials.Instance?.ShowGoalSuccess(_tutorialFade);
        yield return WaitForDialogue(_successDialogueDuration);
        _tutorialFade?.FadeOut();
        yield return WaitForDialogue(_completionDelay);
        _routine = null;
        onTutorialDone?.Invoke();
    }

    void SetActionsEnabled(bool value)
    {
        _grandpaCpu.enabled = false;
        _grandpaCpu.StopAllCoroutines();
        foreach (ActionState state in _actionStates)
        {
            state.Actions.ResetActionState();
            state.Actions.CanKick = value;
            state.Actions.DisableInput = !value;
        }
        _grandpaCpu.enabled = value;
    }

    void OnGoal(GoalEvent payload)
    {
        if (!_roundActive)
            return;

        _succeeded = payload.ScoringSideData.SideType == FieldSideType.Left && _playerHasTouchedBall;
        EndRound();
    }

    void OnOut(OutEvent payload)
    {
        if (_roundActive)
            EndRound();
    }

    void EndRound()
    {
        _roundActive = false;
        _returningToPlayer = false;
        _ballManager.SuppressAutomaticReset = true;
        GoalsManager.Instance?.SetCollidersEnabled(false);
        // Stop any pending CPU reflex before another ball contact can start a return.
        _grandpaCpu.enabled = false;
        _grandpaCpu.StopAllCoroutines();
    }

    void OnBallContactedPlayer(PlayerActions actions, Collider2D bodyCollider)
    {
        if (!_roundActive)
            return;
        if (actions == _player)
        {
            _playerHasTouchedBall = true;
            _returningToPlayer = false;
            return;
        }
        if (actions != _grandpaActions || _returningToPlayer || !actions.IsWithinActionContactWindow)
            return;
        if (!actions.IsKickingContact(bodyCollider) && bodyCollider != actions.HeadContactCollider)
            return;

        Rigidbody2D body = _ball.Rigidbody;
        float direction = Mathf.Sign(_player.transform.position.x - body.position.x);
        // Guide an actual block/kick back; a shot still traveling past Grandpa remains in play.
        if (body.linearVelocity.x * direction <= 0f)
            return;

        float targetX = _player.transform.position.x + _player.AttackingDirection * _receiveDistance;
        if ((targetX - body.position.x) * direction <= 0f)
        {
            // The learner is already within receiving range; leave the ball there to kick.
            body.linearVelocity = new Vector2(0f, Mathf.Min(0f, body.linearVelocity.y));
            _returningToPlayer = true;
            return;
        }
        float groundY = FindGroundHeight(targetX);
        float targetY = groundY + _ball.Collider.bounds.extents.y + 0.35f;
        float gravity = Physics2D.gravity.y * body.gravityScale;
        if (gravity >= -Mathf.Epsilon)
            return;

        float peakY = Mathf.Max(body.position.y, targetY) + _returnArcHeight;
        float duration = Mathf.Sqrt(2f * (peakY - body.position.y) / -gravity) +
                         Mathf.Sqrt(2f * (peakY - targetY) / -gravity);
        duration = Mathf.Max(duration, Mathf.Abs(targetX - body.position.x) / _maximumReturnSpeed);
        Vector2 velocity = new Vector2((targetX - body.position.x) / duration,
            (targetY - body.position.y) / duration - 0.5f * gravity * duration);
        body.linearVelocity = velocity;
        _returningToPlayer = true;
    }

    void FixedUpdate()
    {
        if (!_roundActive || !_returningToPlayer || _ball == null || _player == null)
            return;
        Rigidbody2D body = _ball.Rigidbody;
        if (body.bodyType != RigidbodyType2D.Dynamic)
        {
            _returningToPlayer = false;
            return;
        }

        // An unanswered return drops in front of the learner instead of becoming a shot past them.
        float direction = -_player.AttackingDirection;
        float stopX = _player.transform.position.x + _player.AttackingDirection * _receiveDistance;
        float distance = Mathf.Max(0f, (stopX - body.position.x) * direction);
        Vector2 velocity = body.linearVelocity;
        float speed = velocity.x * direction;
        if (speed > 0f)
        {
            velocity.x = direction * Mathf.Min(speed, distance / Time.fixedDeltaTime);
            body.linearVelocity = velocity;
        }
    }

    float FindGroundHeight(float x)
    {
        float highest = float.NegativeInfinity;
        Vector2 start = new Vector2(x, Mathf.Max(_ball.transform.position.y, _player.transform.position.y) + 5f);
        foreach (RaycastHit2D hit in Physics2D.RaycastAll(start, Vector2.down, 100f))
        {
            if (hit.collider != null && hit.collider.CompareTag("Ground") && hit.normal.y > 0.5f)
                highest = Mathf.Max(highest, hit.point.y);
        }
        return float.IsNegativeInfinity(highest) ? _player.transform.position.y - 1f : highest;
    }

    static IEnumerator WaitForDialogue(float seconds)
    {
        float elapsed = 0f;
        while (elapsed < seconds)
        {
            if (Time.timeScale > 0f)
                elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
    }

    void OnDisable()
    {
        if (_routine != null)
            StopCoroutine(_routine);
        _routine = null;
        if (!_active)
            return;

        _active = false;
        _roundActive = false;
        EventBus<GoalEvent>.OnEvent -= OnGoal;
        EventBus<OutEvent>.OnEvent -= OnOut;
        if (_ball != null)
            _ball.ContactedPlayer -= OnBallContactedPlayer;
        if (_match != null)
            _match.SuppressGoalAndOutEvents = _previousMatchSuppression;
        if (_ballManager != null)
        {
            _ballManager.SuppressAutomaticReset = _previousResetSuppression;
            _ballManager.ResetPositionOverride = _previousResetPosition;
        }
        if (_grandpaCpu != null)
        {
            _grandpaCpu.EndTutorialPractice();
            _grandpaCpu.enabled = _previousCpuEnabled;
        }
        foreach (ActionState state in _actionStates)
        {
            if (state.Actions == null)
                continue;
            state.Actions.ResetActionState();
            state.Actions.CanKick = state.CanKick;
            state.Actions.DisableInput = state.DisableInput;
            state.Actions.ForcedToHoldKick = state.ForcedToHoldKick;
            state.Actions.LockMovementToFacingDirection = state.LockMovement;
        }
        _actionStates.Clear();
        GoalsManager.Instance?.SetCollidersEnabled(true);
    }
}
