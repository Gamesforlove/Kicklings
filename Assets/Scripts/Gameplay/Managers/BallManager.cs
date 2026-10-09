using CommonDataTypes;
using Gameplay.Spawners;
using UnityEngine;

namespace Gameplay.Managers
{
    public class BallManager : MonoBehaviour
    {
        const float ResetWarningDuration = 2f;
        const float ResetWarningStartFrequency = 2f;
        const float ResetWarningEndFrequency = 8f;

        [SerializeField] BallSpawner _ballSpawner;
        [Header("Hard Inaccessible-Ball Fallback")]
        [SerializeField, Min(0f)] float _unreachableFallbackDelay = 10f;
        [SerializeField, Min(0f)] float _dropFallbackGracePeriod = 12f;
        [SerializeField, Min(0f)] float _reachableHorizontalLimit = 8.75f;
        [SerializeField] float _minimumReachableHeight = -3f;
        [SerializeField] float _maximumReachableHeight = 4.5f;
        [SerializeField, Min(0f)] float _reachabilityPredictionTime = 1.25f;
        [SerializeField, Range(1, 8)] int _reachabilityPredictionSteps = 4;

        float _lastPlayerTouchTime;
        bool _awaitingFirstTouchAfterDrop;
        float _resetWarningStartTime = -1f;
        SpriteRenderer _resetWarningRenderer;
        bool _resetWarningRendererWasEnabled;
        
        public static BallManager Instance { get; private set; }
        void Awake()
        {
            Instance = this;
            MarkBallDropped();
        }

        void OnEnable() => BallScript.TouchedPlayer += OnBallTouchedPlayer;
        void OnDisable()
        {
            BallScript.TouchedPlayer -= OnBallTouchedPlayer;
            CancelResetWarning();
        }

        void Update()
        {
            if ((MatchManager.Instance != null && MatchManager.Instance.MatchDone) ||
                Ball == null || Ball.Rigidbody == null ||
                Ball.Rigidbody.bodyType != RigidbodyType2D.Dynamic ||
                !Ball.Rigidbody.simulated)
            {
                CancelResetWarning();
                return;
            }

            float untouchedTime = Time.time - _lastPlayerTouchTime;
            float fallbackDelay = _awaitingFirstTouchAfterDrop
                ? _dropFallbackGracePeriod
                : _unreachableFallbackDelay;
            bool needsResetWarning = fallbackDelay > 0f &&
                                     untouchedTime >= Mathf.Max(0f, fallbackDelay - ResetWarningDuration) &&
                                     !HasReachablePredictedPosition();
            if (!needsResetWarning)
            {
                CancelResetWarning();
                return;
            }

            if (_resetWarningStartTime < 0f)
            {
                _resetWarningStartTime = Time.time;
                _resetWarningRenderer = Ball.Renderer;
                _resetWarningRendererWasEnabled = _resetWarningRenderer != null &&
                                                  _resetWarningRenderer.enabled;
            }

            float warningElapsed = Time.time - _resetWarningStartTime;
            // Always give the full warning, even if the ball becomes unreachable after the timeout.
            if (warningElapsed >= ResetWarningDuration && untouchedTime >= fallbackDelay)
            {
                ResetBall();
                return;
            }

            if (_resetWarningRenderer != null)
            {
                float progress = Mathf.Clamp01(warningElapsed / ResetWarningDuration);
                // Integrate the rising blink frequency so each flash gets progressively faster.
                float blinkCycles = ResetWarningDuration *
                                    (ResetWarningStartFrequency * progress +
                                     0.5f * (ResetWarningEndFrequency - ResetWarningStartFrequency) *
                                     progress * progress);
                _resetWarningRenderer.enabled = _resetWarningRendererWasEnabled &&
                                               Mathf.Repeat(blinkCycles, 1f) < 0.5f;
            }
        }

        public BallScript Ball { get; private set; }
    
        public void SpawnBall()
        {
            CancelResetWarning();
            Ball = _ballSpawner?.SpawnBall();
            ResetBallWithSpin(FieldSideType.Left);
        }
    
        public void ResetBall()
        {
            MarkBallDropped();
            _ballSpawner?.ResetBall();
            Ball?.Reset();
        }

        public void ResetBall(FieldSideType sideType)
        {
            MarkBallDropped();
            _ballSpawner?.ResetBallOnSide(sideType);
            Ball?.Reset();
        }

        public void ResetBallWithSpin(FieldSideType sideType)
        {
            MarkBallDropped();
            _ballSpawner?.ResetBall();
            Ball?.ResetWithSpin(sideType);
        }

        void OnBallTouchedPlayer()
        {
            CancelResetWarning();
            _awaitingFirstTouchAfterDrop = false;
            _lastPlayerTouchTime = Time.time;
        }

        void MarkBallDropped()
        {
            CancelResetWarning();
            _awaitingFirstTouchAfterDrop = true;
            _lastPlayerTouchTime = Time.time;
        }

        void CancelResetWarning()
        {
            if (_resetWarningStartTime < 0f)
                return;

            if (_resetWarningRenderer != null)
                _resetWarningRenderer.enabled = _resetWarningRendererWasEnabled;

            _resetWarningStartTime = -1f;
            _resetWarningRenderer = null;
        }

        bool HasReachablePredictedPosition()
        {
            int predictionSteps = Mathf.Max(1, _reachabilityPredictionSteps);
            Vector2 startPosition = Ball.Rigidbody.position;
            Vector2 startVelocity = Ball.Rigidbody.linearVelocity;
            float gravity = Physics2D.gravity.y * Ball.Rigidbody.gravityScale;

            for (int i = 0; i <= predictionSteps; i++)
            {
                float time = _reachabilityPredictionTime * i / predictionSteps;
                Vector2 predictedPosition = startPosition + startVelocity * time;
                predictedPosition.y += 0.5f * gravity * time * time;
                if (IsInsideReachableEnvelope(predictedPosition))
                    return true;
            }

            return false;
        }

        bool IsInsideReachableEnvelope(Vector2 position)
        {
            return Mathf.Abs(position.x) <= _reachableHorizontalLimit &&
                   position.y >= _minimumReachableHeight &&
                   position.y <= _maximumReachableHeight;
        }
    }
}
