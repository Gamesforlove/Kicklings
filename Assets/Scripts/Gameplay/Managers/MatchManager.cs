using CommonDataTypes;
using DG.Tweening;
using EventBusSystem;
using Gameplay.CharacterComponents.Cpu;
using Gameplay.Spawners;
using Scene_Management;
using System;
using System.Collections;
using UI.MainMenu.TournamentMode;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

namespace Gameplay.Managers
{
    public class MatchManager : MonoBehaviour
    {
        [SerializeField] UiManager _uiManager;
        [SerializeField] PlayersManager _playersManager;
        [SerializeField] BallManager _ballManager;
        [SerializeField] GoalsManager _goalsManager;
        [SerializeField] AbilityTestingManager _abilityTestingManager;
        [SerializeField] FieldSideData _leftSideData;
        [SerializeField] FieldSideData _rightSideData;
        
        Match _match;
        float _matchStartTime;
        MatchEndedEvent _pendingMatchEndedEvent;

        int _leftScore, _rightScore;
        public void ResetGame()
        {
            _leftScore = 0;
            _rightScore = 0;
            _uiManager?.ChangeScore(_leftScore, _rightScore);
            _playersManager?.ResetPlayers();
            _playersManager?.EnablePlayers();
            _ballManager?.ResetBallWithSpin(FieldSideType.Left);
            _goalsManager?.SetCollidersEnabled(true);
            TimeScaleManager.SetGameplayTimeScale();
        }

        public void EndGame(bool retried = false)
        {
#if UNITY_EDITOR == false
            if (_pendingMatchEndedEvent != null)
            {
                _pendingMatchEndedEvent.Retried = retried;
                AnalyticsService.Instance.RecordEvent(_pendingMatchEndedEvent); // sends to unity dashboard, ask Rishi
                Debug.Log("sent match ended event to analytics");
                _pendingMatchEndedEvent = null;
            }
#endif

            DOTween.KillAll();

            TimeScaleManager.SetDefaultTimeScale();
            EventBus<OnLoadScene>.Raise(new OnLoadScene(SceneName.MainMenu));
        }

        async void Start()
        {
            _match = MatchFlow.Match;

            if (_match is TournamentMatch tournamentMatch)
            {
                DifficultyLevel difficulty = tournamentMatch.Tournament.GetDifficultyForRound();
                _playersManager?.SetDifficulty(difficulty);
            }

            _playersManager?.SpawnEntities(_match.Settings);
            _abilityTestingManager?.SetUpAbilityActors(_playersManager.GetAbilityActors());
            _ballManager?.SpawnBall();
            _goalsManager?.SetCollidersEnabled(true);
            _leftScore = 0;
            _rightScore = 0;
            _matchStartTime = Time.time;
            TimeScaleManager.SetGameplayTimeScale();

#if UNITY_EDITOR == false
            try
            {
                await UnityServices.InitializeAsync();
                AnalyticsService.Instance.StartDataCollection();
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Analytics init failed: {e.Message}");
            }
#endif
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

        void OnGoalEvent(GoalEvent payload)
        {
            ChangeScore(payload.ScoringSideData.SideType);
            _goalsManager.SetCollidersEnabled(false);
            StartCoroutine(OnGoalEventRoutine(payload));
        }

        void OnOutEvent(OutEvent payload)
        {
            StartCoroutine(OnOutEventRoutine(payload));
        }

        IEnumerator OnGoalEventRoutine(GoalEvent payload)
        {
            TimeScaleManager.SlowMotion();
            
            yield return StartCoroutine(_uiManager.ShowGoalNotification(payload));
            
            TimeScaleManager.SetGameplayTimeScale();
            
            if (_leftScore >= _match.Settings.GoalsToEndMatch || _rightScore >= _match.Settings.GoalsToEndMatch)
            {
                ShowEndgame(payload);
                yield break;
            }
            
            RespawnGameplayElements(payload.ScoredSideData.SideType);
        }
    
        IEnumerator OnOutEventRoutine(OutEvent payload)
        {
            TimeScaleManager.SlowMotion();
            yield return StartCoroutine(_uiManager.ShowOutNotification(payload));
            TimeScaleManager.SetGameplayTimeScale();
            RespawnGameplayElements(payload.FieldSideData.SideType);
        }

        void RespawnGameplayElements(FieldSideType sideType)
        {
            _playersManager.ResetPlayers();
            _ballManager.ResetBallWithSpin(sideType);
            //_ballManager.ResetBall();
            _goalsManager.SetCollidersEnabled(true);
        }
        void ShowEndgame(GoalEvent payload)
        {
            //TimeScaleManager.PauseGame();

            var data = GameAndPlayerData.Instance;
            if (data != null)
            {
                data.numGamesPlayed++;
                data.numGamesPlayedToday++;
                data.totalPlaytime += Time.time - _matchStartTime;

                if (_match.IsPlayerWinner)
                {
                    data.numGamesWon++;
                    data.numGamesWonToday++;
                }
                else
                {
                    data.numGamesLost++;
                    data.numGamesLostToday++;
                }
                data.UpdateElo(_match.IsPlayerWinner);
            }

#if UNITY_EDITOR == false
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                Debug.Log("Analytics was not initialized by the time the match ended. Event not sent.");
            }
            else
            {
                var analyticsEvent = new MatchEndedEvent
                {
                    Mode = _match.Settings.IsTournamentMatch ? "tournament" : "regular",
                    Difficulty = (int)(PlayersSpawner.Instance ? PlayersSpawner.Instance.CurrentDifficulty : 0),
                    EndReason = _match.IsPlayerWinner ? "win" : "lose",
                    MatchDuration = Mathf.RoundToInt(Time.time - _matchStartTime),
                    PlayerSkillRating = Mathf.RoundToInt(100f * (data != null ? data.T : 0.35f))
                };

                if (_match.IsPlayerWinner)
                {
                    AnalyticsService.Instance.RecordEvent(analyticsEvent);
                    //Debug.Log("sent match ended event to analytics");
                }
                else
                {
                    // On loss, event only gets sent once we choose retry or quit.
                    _pendingMatchEndedEvent = analyticsEvent;
                }
            }
#endif

            _playersManager.DisablePlayers();
            _match.HandleEndgameUI(this, _uiManager, payload);
        }

        public void InstantWin()
        {
            //TimeScaleManager.PauseGame();
            GoalEvent payload = new GoalEvent(_leftSideData, _rightSideData);
            _playersManager.DisablePlayers();
            _match.HandleEndgameUI(this, _uiManager, payload);
        }
        public void InstantLose()
        {
            //TimeScaleManager.PauseGame();
            GoalEvent payload = new GoalEvent(_rightSideData, _leftSideData);
            _playersManager.DisablePlayers();
            _match.HandleEndgameUI(this, _uiManager, payload);
        }

        void ChangeScore(FieldSideType scoringSide)
        {
            switch (scoringSide)
            {
                case FieldSideType.Right:
                    _rightScore++;
                    break;
                case FieldSideType.Left:
                    _leftScore++;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(scoringSide), scoringSide, null);
            }
            
            _uiManager.ChangeScore(_leftScore, _rightScore);
        }
    }
}