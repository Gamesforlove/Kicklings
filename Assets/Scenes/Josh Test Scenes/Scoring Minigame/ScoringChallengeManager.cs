using System;
using System.Collections;
using CommonDataTypes;
using EventBusSystem;
using Scene_Management;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.Managers
{
    public class ScoringChallengeManager : MonoBehaviour
    {
        [SerializeField] UiManager _uiManager;
        [SerializeField] PlayersManager _playersManager;  
        [SerializeField] BallLauncher _ballLauncher;      
        [SerializeField] GoalsManager _goalsManager;       
        [Header("Challenge Settings")]
        [SerializeField] int _scoreTarget = 10;
        [SerializeField] float _challengeDuration = 60f;
        [SerializeField] MinigameCompletionUI minigameCompletionUI;

        int _score;
        float _timeRemaining;
        bool _challengeActive;
        bool _playerSpawned;

        public static ScoringChallengeManager Instance { get; private set; }
        private void Awake() => Instance = this;

        public bool ChallengeDone { get; private set; }

        void OnEnable()
        {
            EventBus<GoalEvent>.OnEvent += OnGoalEvent;
            EventBus<OutEvent>.OnEvent += OnOutEvent;
        }

        void OnDisable()
        {
            EventBus<GoalEvent>.OnEvent -= OnGoalEvent;
            EventBus<OutEvent>.OnEvent -= OnOutEvent;
        }

        public void StartChallenge()
        {
            // Spawned here, not in Start() - MinigameMatchBuilder sets enabled = true and
            // calls StartChallenge() in the same method, before Unity's next Start() pass
            // would fire. Spawning here guarantees the player exists before anything else
            // (timer, launcher, goal colliders) goes live, instead of racing a frame behind.
            if (!_playerSpawned)
            {
                _playersManager?.SpawnSinglePlayer();
                _playerSpawned = true;
            }
            else
            {
                // Later rounds: the player already exists and has fully initialized,
                // so reset it back to its spawn position/pose.
                _playersManager?.ResetPlayers();
            }

            _score = 0;
            _timeRemaining = _challengeDuration;
            _challengeActive = true;
            ChallengeDone = false;

            _uiManager?.ChangeChallengeScore(_score, _scoreTarget);
            _uiManager?.InitializeChallengeTimer(_challengeDuration);
            _uiManager?.UpdateTimer(_timeRemaining);

            _goalsManager?.SetCollidersEnabled(true);
            if (_ballLauncher != null) _ballLauncher.autoLaunch = true;

            TimeScaleManager.SetGameplayTimeScale();
        }

        void Update()
        {
            if (!_challengeActive) return;

            _timeRemaining -= Time.deltaTime;
            _uiManager?.UpdateTimer(Mathf.Max(_timeRemaining, 0f));

            if (_timeRemaining <= 0f)
            {
                EndChallenge(won: false);
                minigameCompletionUI.ShowCompletionGoal(_score);
            }
        }

        #region OnGoal
        void OnGoalEvent(GoalEvent payload)
        {
            if (!_challengeActive) return;

            _score++;
            _uiManager?.ChangeChallengeScore(_score, _scoreTarget);

            if (_score >= _scoreTarget)
            {
                EndChallenge(won: true);
                minigameCompletionUI.ShowCompletionGoal(_score);
                return;
            }
            
            _goalsManager.SetCollidersEnabled(false);
            StartCoroutine(OnGoalEventRoutine(payload));
            
        }

        IEnumerator OnGoalEventRoutine(GoalEvent payload)
        {
            TimeScaleManager.SlowMotion();
            yield return StartCoroutine(_uiManager.ShowGoalNotification(payload));
            TimeScaleManager.SetGameplayTimeScale();
            _playersManager.ResetPlayers();
            _ballLauncher.DespawnAllBall();
            _goalsManager.SetCollidersEnabled(true);
        }
        #endregion

        #region OnOut
        void OnOutEvent(OutEvent payload)
        {
            if (!_challengeActive) return;
            StartCoroutine(OnOutEventRoutine(payload));
        }

        IEnumerator OnOutEventRoutine(OutEvent payload)
        {
            TimeScaleManager.SlowMotion();
            yield return StartCoroutine(_uiManager.ShowOutNotification(payload));
            TimeScaleManager.SetGameplayTimeScale();
        }
        #endregion

        void EndChallenge(bool won)
        {
            _challengeActive = false;
            ChallengeDone = true;

            if (_ballLauncher != null) _ballLauncher.autoLaunch = false;
            _goalsManager?.SetCollidersEnabled(false);

            TimeScaleManager.PauseGame();
            _uiManager?.ShowChallengeResult(won, _score, _scoreTarget);
        }

        public void ExitToMenu()
        {
            TimeScaleManager.SetDefaultTimeScale();
            EventBus<OnLoadScene>.Raise(new OnLoadScene(SceneName.MainMenu));
        }
    }
}