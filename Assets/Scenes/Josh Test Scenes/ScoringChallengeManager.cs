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

        int _score;
        float _timeRemaining;
        bool _challengeActive;

        public static ScoringChallengeManager Instance { get; private set; }
        private void Awake() => Instance = this;

        public bool ChallengeDone { get; private set; }

        void Start()
        {
            _playersManager?.SpawnSinglePlayer();
            StartChallenge();
        }

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
            _score = 0;
            _timeRemaining = _challengeDuration;
            _challengeActive = true;
            ChallengeDone = false;

            _uiManager?.ChangeChallengeScore(_score, _scoreTarget);
            _uiManager?.InitializeChallengeTimer(_challengeDuration);
            _uiManager?.UpdateTimer(_timeRemaining);
            
            _playersManager?.ResetPlayers();

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
                return;
            }

            StartCoroutine(OnGoalEventRoutine(payload));
        }

        IEnumerator OnGoalEventRoutine(GoalEvent payload)
        {
            TimeScaleManager.SlowMotion();
            yield return StartCoroutine(_uiManager.ShowGoalNotification(payload));
            TimeScaleManager.SetGameplayTimeScale();
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