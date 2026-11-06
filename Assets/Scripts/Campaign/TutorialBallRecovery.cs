using Gameplay.CharacterComponents;
using Gameplay.Managers;
using UnityEngine;

public class TutorialBallRecovery : MonoBehaviour
{
    [Tooltip("Game seconds without a learner touch before resetting; includes the warning blink.")]
    [SerializeField, Min(0f)] float _untouchedResetDelay = 8f;
    [Tooltip("Game seconds after a drop allowed for the learner's first touch; includes the warning blink.")]
    [SerializeField, Min(0f)] float _dropGracePeriod = 10f;
    [SerializeField, Min(0.1f)] float _warningDuration = 2f;

    PlayerActions _player;
    BallManager _ballManager;
    BallScript _ball;
    bool _active;
    bool _playing;
    bool _previousResetSuppression;
    bool _awaitingFirstTouch;
    float _lastPlayerTouchTime;
    float _warningStart = -1f;
    SpriteRenderer _warningRenderer;
    bool _rendererWasEnabled;

    public void BeginPractice(PlayerActions player)
    {
        EndPractice();
        _ballManager = BallManager.Instance;
        _ball = _ballManager != null ? _ballManager.Ball : null;
        if (!isActiveAndEnabled || player == null || _ballManager == null || _ball == null)
            return;

        _player = player;
        _previousResetSuppression = _ballManager.SuppressAutomaticReset;
        _ballManager.SuppressAutomaticReset = true;
        _ball.ContactedPlayer += OnPlayerContact;
        _active = true;
        NotifyBallDropped();
    }

    public void SetPlaying(bool value)
    {
        _playing = _active && value;
        if (!_playing)
            CancelWarning();
    }

    public void NotifyBallDropped()
    {
        CancelWarning();
        _awaitingFirstTouch = true;
        _lastPlayerTouchTime = Time.time;
    }

    void OnPlayerContact(PlayerActions actions, Collider2D bodyCollider)
    {
        if (!_playing || actions != _player)
            return;
        CancelWarning();
        _awaitingFirstTouch = false;
        _lastPlayerTouchTime = Time.time;
    }

    void Update()
    {
        if (!_playing || Time.timeScale <= 0f || _ball == null || _ballManager == null ||
            _ball.Rigidbody.bodyType != RigidbodyType2D.Dynamic || !_ball.Rigidbody.simulated)
            return;

        float delay = _awaitingFirstTouch ? _dropGracePeriod : _untouchedResetDelay;
        float elapsed = Time.time - _lastPlayerTouchTime;
        if (delay <= 0f || elapsed < Mathf.Max(0f, delay - _warningDuration))
        {
            CancelWarning();
            return;
        }
        if (_warningStart < 0f)
        {
            _warningStart = Time.time;
            _warningRenderer = _ball.Renderer;
            _rendererWasEnabled = _warningRenderer != null && _warningRenderer.enabled;
        }

        float warningElapsed = Time.time - _warningStart;
        if (warningElapsed >= _warningDuration && elapsed >= delay)
        {
            CancelWarning();
            _ballManager.ResetBall();
            NotifyBallDropped();
            return;
        }
        if (_warningRenderer != null)
        {
            float progress = Mathf.Clamp01(warningElapsed / _warningDuration);
            float blinkCycles = _warningDuration * (2f * progress + 3f * progress * progress);
            _warningRenderer.enabled = _rendererWasEnabled && Mathf.Repeat(blinkCycles, 1f) < 0.5f;
        }
    }

    void CancelWarning()
    {
        if (_warningStart < 0f)
            return;
        if (_warningRenderer != null)
            _warningRenderer.enabled = _rendererWasEnabled;
        _warningStart = -1f;
        _warningRenderer = null;
    }

    public void EndPractice()
    {
        SetPlaying(false);
        if (!_active)
            return;
        _active = false;
        if (_ball != null)
            _ball.ContactedPlayer -= OnPlayerContact;
        if (_ballManager != null)
            _ballManager.SuppressAutomaticReset = _previousResetSuppression;
        _player = null;
        _ball = null;
        _ballManager = null;
    }

    void OnDisable() => EndPractice();
}
