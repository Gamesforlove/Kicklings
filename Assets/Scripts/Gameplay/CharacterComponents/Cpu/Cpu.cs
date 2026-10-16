using System;
using System.Collections;
using Gameplay.Managers;
using UnityEngine;
using static Gameplay.Spawners.PlayersSpawner;

namespace Gameplay.CharacterComponents.Cpu
{
    [Serializable]
    public class CpuConfiguration
    {
        public EntityData EntityData;
        public CpuDifficultyPreset.DifficultySettings DifficultySettings;

        public CpuConfiguration(EntityData entityData, CpuDifficultyPreset.DifficultySettings difficultySettings)
        {
            EntityData = entityData;
            DifficultySettings = difficultySettings;
        }
    }

    public class Cpu : Entity
    {
        CpuDifficultyPreset.DifficultySettings _difficultySettings;
        
        BallManager _ballManager;
        BallScript _ball;

        CountdownTimer _actionTimer;
        
        BallProximityChecker _ballProximityChecker;
        bool _tutorialPractice;
        bool _tutorialReactionPending;
        float _nextTutorialActionTime;
        CpuDifficultyPreset.DifficultySettings _previousDifficultySettings;
        Vector2 _tutorialMovementLimits;
        float _tutorialApproachDistance;

        public void BeginTutorialPractice(CpuDifficultyPreset.DifficultySettings settings,
            Vector2 movementLimits, float approachDistance)
        {
            if (_tutorialPractice || settings == null)
                return;

            _previousDifficultySettings = _difficultySettings;
            _difficultySettings = settings;
            _tutorialPractice = true;
            _tutorialReactionPending = false;
            _nextTutorialActionTime = Time.time;
            _tutorialMovementLimits = new Vector2(Mathf.Min(movementLimits.x, movementLimits.y),
                Mathf.Max(movementLimits.x, movementLimits.y));
            _tutorialApproachDistance = Mathf.Max(0f, approachDistance);
            StopAllCoroutines();
            _ballManager = BallManager.Instance;
            _ballProximityChecker = GetComponent<BallProximityChecker>();
            _ballProximityChecker?.SetUp(settings.ProximityPoints);
        }

        public void EndTutorialPractice()
        {
            if (!_tutorialPractice)
                return;

            StopAllCoroutines();
            _tutorialPractice = false;
            _tutorialReactionPending = false;
            _difficultySettings = _previousDifficultySettings;
            if (_difficultySettings != null)
                _ballProximityChecker?.SetUp(_difficultySettings.ProximityPoints);
            _actionTimer?.Reset();
        }

        public void SetUp(CpuConfiguration config, PlayerType type)
        {
            if (config == null) return;

            base.SetUp(config.EntityData, type);
            _difficultySettings = config.DifficultySettings;
            
            if (_difficultySettings != null)
                _ballProximityChecker?.SetUp(_difficultySettings.ProximityPoints);
            
            _actionTimer = new CountdownTimer(_difficultySettings.TimeBetweenKicks.RandomValue);
            if (_actionTimer != null)
            {
                _actionTimer.OnTimerStop += DoAction;
                _actionTimer.Start();
            }
        }


        public override void Reset()
        {
            base.Reset();
            StopAllCoroutines();
            _actionTimer?.Reset();
            _tutorialReactionPending = false;
            _nextTutorialActionTime = Time.time;
        }

        void Start()
        {
            _ballProximityChecker = GetComponent<BallProximityChecker>();
            _ballManager = BallManager.Instance;
        }

        void Update()
        {
            if (_tutorialPractice)
            {
                if (!_tutorialReactionPending && Time.time >= _nextTutorialActionTime &&
                    TryGetTutorialMovementTarget(out Vector2 movementTarget))
                    StartCoroutine(TutorialReflex());
                return;
            }

            CpuPlayer();
            _actionTimer?.Tick(Time.deltaTime);
        }

        bool TryGetTutorialMovementTarget(out Vector2 target)
        {
            target = default;
            BallScript ball = _ballManager != null ? _ballManager.Ball : null;
            if (Time.timeScale <= 0f || PlayerActions == null || !PlayerActions.CanReceivePlayerAction ||
                Rigidbody == null || ball == null || ball.Rigidbody.bodyType != RigidbodyType2D.Dynamic)
                return false;

            float predictionTime = EntityData?.KickAssist?.BallPredictionTime ?? 0.35f;
            Vector2 predictedPosition = ball.Rigidbody.position + ball.Rigidbody.linearVelocity * predictionTime;
            predictedPosition.y += 0.5f * Physics2D.gravity.y * ball.Rigidbody.gravityScale *
                                   predictionTime * predictionTime;
            target = new Vector2(Mathf.Clamp(predictedPosition.x,
                _tutorialMovementLimits.x, _tutorialMovementLimits.y), predictedPosition.y);

            if (_ballProximityChecker != null &&
                _ballProximityChecker.IsBallWithinRange(ball.Rigidbody, minimumSpeedMultiplier: 1f))
                return true;

            // Move into an incoming shot with the same physical hop used by the player.
            // Let a return travel to the learner rather than chasing it across the field.
            bool inDefendingArea = (predictedPosition.x >= _tutorialMovementLimits.x &&
                                    predictedPosition.x <= _tutorialMovementLimits.y) ||
                                   (ball.Rigidbody.position.x >= _tutorialMovementLimits.x &&
                                    ball.Rigidbody.position.x <= _tutorialMovementLimits.y);
            bool movingAway = ball.Rigidbody.linearVelocity.x * PlayerActions.AttackingDirection > 0.5f;
            bool approaching = (ball.Rigidbody.position.x - Rigidbody.position.x) *
                               ball.Rigidbody.linearVelocity.x < 0f;
            bool needsHorizontalReach = Mathf.Abs(target.x - Rigidbody.position.x) > 0.4f;
            bool needsBlockHop = target.y > Rigidbody.position.y + 0.5f;
            return inDefendingArea && !movingAway &&
                   Mathf.Abs(target.x - Rigidbody.position.x) <= _tutorialApproachDistance &&
                   (needsHorizontalReach || needsBlockHop || approaching);
        }

        IEnumerator TutorialReflex()
        {
            _tutorialReactionPending = true;
            yield return new WaitForSeconds(_difficultySettings.ReactionTime.RandomValue);
            if (TryGetTutorialMovementTarget(out Vector2 target))
            {
                PlayerActions.PerformTutorialAction(target);
                yield return new WaitForSeconds(0.3f);
                PlayerActions.OnActionCancelled();
            }
            _nextTutorialActionTime = Time.time + _difficultySettings.TimeBetweenKicks.RandomValue;
            _tutorialReactionPending = false;
        }
    
        void CpuPlayer()
        {
            if (_ballManager != null && _ballManager.Ball != null && _ballProximityChecker != null &&
                _ballProximityChecker.IsBallWithinRange(_ballManager.Ball.Rigidbody))
            {
                DoAction();
            }

        }
        
        void DoAction()
        {
            StartCoroutine(RandomReflex(_difficultySettings.ReactionTime.RandomValue));
            _actionTimer.Reset(_difficultySettings.TimeBetweenKicks.RandomValue);
            _actionTimer.Start();
        }
        
        IEnumerator RandomReflex(float time)
        {
            yield return new WaitForSeconds(time);
            PlayerActions.OnActionPerformed();
            yield return new WaitForSeconds(0.3f); // TODO: Maybe randomize this too, or make it a setting in the difficulty preset
            PlayerActions.OnActionCancelled();
        }
    }
}
