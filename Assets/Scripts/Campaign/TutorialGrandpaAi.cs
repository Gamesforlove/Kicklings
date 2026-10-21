using System.Collections;
using Gameplay.CharacterComponents;
using Gameplay.CharacterComponents.Cpu;
using Gameplay.Managers;
using UnityEngine;

public class TutorialGrandpaAi : MonoBehaviour
{
    [SerializeField] CpuDifficultyPreset _difficultyPreset;
    [SerializeField] DifficultyLevel _difficulty = DifficultyLevel.Easy1;
    [SerializeField] CpuDifficultyPreset.FloatRange _timeBetweenActions =
        new CpuDifficultyPreset.FloatRange { Min = 0.9f, Max = 1.8f };
    [Tooltip("Horizontal area Grandpa can move toward when defending the goal.")]
    [SerializeField] Vector2 _movementLimits = new Vector2(1.5f, 8.5f);
    [Tooltip("How far ahead Grandpa starts moving toward an approaching ball.")]
    [SerializeField, Min(0f)] float _approachDistance = 4.5f;

    Cpu _cpu;
    PlayerActions _actions;
    Rigidbody2D _body;
    BallManager _ballManager;
    BallProximityChecker _proximityChecker;
    CpuDifficultyPreset.DifficultySettings _settings;
    CpuDifficultyPreset.ProximityPoint[] _previousProximityPoints;
    bool _previousCpuEnabled;
    bool _active;
    bool _playing;
    bool _reactionPending;
    float _nextActionTime;

    public bool BeginPractice(PlayerActions actions)
    {
        if (_active)
            EndPractice();

        _actions = actions;
        _cpu = actions != null ? actions.GetComponent<Cpu>() : null;
        _body = actions != null ? actions.GetComponent<Rigidbody2D>() : null;
        _proximityChecker = actions != null ? actions.GetComponent<BallProximityChecker>() : null;
        _ballManager = BallManager.Instance;
        if (_actions == null || _cpu == null || _body == null || _difficultyPreset == null)
        {
            Debug.LogError("Grandpa's tutorial AI needs his actions, CPU, body, and difficulty preset.", this);
            return false;
        }

        CpuDifficultyPreset.DifficultySettings source = _difficultyPreset.GetSettingsForDifficulty(_difficulty);
        if (source == null)
        {
            Debug.LogError("Grandpa's tutorial AI needs valid difficulty settings.", this);
            return false;
        }
        _settings = new CpuDifficultyPreset.DifficultySettings
        {
            ReactionTime = source.ReactionTime,
            ProximityPoints = source.ProximityPoints,
            TimeBetweenKicks = _timeBetweenActions
        };
        _previousCpuEnabled = _cpu.enabled;
        _previousProximityPoints = _proximityChecker != null ? _proximityChecker.ProximityPoints : null;
        _cpu.StopAllCoroutines();
        _cpu.enabled = false;
        _proximityChecker?.SetUp(_settings.ProximityPoints);
        _active = true;
        SetPlaying(false);
        return true;
    }

    public void SetPlaying(bool value)
    {
        StopAllCoroutines();
        _reactionPending = false;
        _nextActionTime = Time.time;
        _playing = _active && value;
    }

    void Update()
    {
        if (_playing && !_reactionPending && Time.time >= _nextActionTime &&
            TryGetMovementTarget(out Vector2 movementTarget))
            StartCoroutine(ReactToBall());
    }

    bool TryGetMovementTarget(out Vector2 target)
    {
        target = default;
        BallScript ball = _ballManager != null ? _ballManager.Ball : null;
        if (Time.timeScale <= 0f || _actions == null || !_actions.CanReceivePlayerAction ||
            _body == null || ball == null || ball.Rigidbody.bodyType != RigidbodyType2D.Dynamic)
            return false;

        float predictionTime = _actions.Data?.KickAssist?.BallPredictionTime ?? 0.35f;
        Vector2 predictedPosition = ball.Rigidbody.position + ball.Rigidbody.linearVelocity * predictionTime;
        predictedPosition.y += 0.5f * Physics2D.gravity.y * ball.Rigidbody.gravityScale *
                               predictionTime * predictionTime;
        float minimumX = Mathf.Min(_movementLimits.x, _movementLimits.y);
        float maximumX = Mathf.Max(_movementLimits.x, _movementLimits.y);
        target = new Vector2(Mathf.Clamp(predictedPosition.x, minimumX, maximumX), predictedPosition.y);

        if (_proximityChecker != null &&
            _proximityChecker.IsBallWithinRange(ball.Rigidbody, minimumSpeedMultiplier: 1f))
            return true;

        bool inDefendingArea = (predictedPosition.x >= minimumX && predictedPosition.x <= maximumX) ||
                               (ball.Rigidbody.position.x >= minimumX && ball.Rigidbody.position.x <= maximumX);
        bool movingAway = ball.Rigidbody.linearVelocity.x * _actions.AttackingDirection > 0.5f;
        bool approaching = (ball.Rigidbody.position.x - _body.position.x) * ball.Rigidbody.linearVelocity.x < 0f;
        bool needsHorizontalReach = Mathf.Abs(target.x - _body.position.x) > 0.4f;
        bool needsBlockHop = target.y > _body.position.y + 0.5f;
        return inDefendingArea && !movingAway &&
               Mathf.Abs(target.x - _body.position.x) <= Mathf.Max(0f, _approachDistance) &&
               (needsHorizontalReach || needsBlockHop || approaching);
    }

    IEnumerator ReactToBall()
    {
        _reactionPending = true;
        yield return new WaitForSeconds(_settings.ReactionTime.RandomValue);
        if (TryGetMovementTarget(out Vector2 target))
        {
            _actions.PerformAssistedAction(target);
            yield return new WaitForSeconds(0.3f);
            _actions.OnActionCancelled();
        }
        _nextActionTime = Time.time + _settings.TimeBetweenKicks.RandomValue;
        _reactionPending = false;
    }

    public void EndPractice()
    {
        SetPlaying(false);
        if (!_active)
            return;

        _active = false;
        if (_proximityChecker != null)
            _proximityChecker.SetUp(_previousProximityPoints);
        if (_cpu != null)
            _cpu.enabled = _previousCpuEnabled;
    }

    void OnDisable() => EndPractice();
}
