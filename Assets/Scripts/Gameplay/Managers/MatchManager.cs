using CommonDataTypes;
using DG.Tweening;
using EventBusSystem;
using Gameplay.CharacterComponents.Cpu;
using Gameplay.Spawners;
using Scene_Management;
using System;
using System.Collections;
using System.Xml.Linq;
using UI.MainMenu.TournamentMode;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Events;

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

        public void SetNewMatch(Match match) { _match = match; if (ranStart) ResetGame();}

        public void EndGame()
        {
            DOTween.KillAll();

            TimeScaleManager.SetDefaultTimeScale();
            EventBus<OnLoadScene>.Raise(new OnLoadScene(SceneName.MainMenu));
        }
        public void EndGame(string transferTo)
        {
            if (Enum.TryParse(transferTo, out SceneName name))
            {
                DOTween.KillAll();

                TimeScaleManager.SetDefaultTimeScale();
                EventBus<OnLoadScene>.Raise(new OnLoadScene(name));
            }
            else
            {
            #if UNITY_EDITOR
                            Debug.LogError("Invalid scene name");
            #endif
            }
        }

        public static MatchManager Instance { get; private set; }
        private void Awake() => Instance = this;

        bool ranStart = false;
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
            ranStart = true;

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

        #region OnGoal
        [Tooltip("Instead of running the normal 'goal scored' code, do this.")]
        public UnityEvent ScriptedGoalEventInstead;
        void OnGoalEvent(GoalEvent payload)
        {
            _goalsManager.SetCollidersEnabled(false);

            if (ScriptedGoalEventInstead != null && ScriptedGoalEventInstead.GetPersistentEventCount() > 0)
            {
                ScriptedGoalEventInstead.Invoke();
            }
            else
            {
                ChangeScore(payload.ScoringSideData.SideType);
                StartCoroutine(OnGoalEventRoutine(payload));
            }
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
        #endregion

        #region OnOut
        void OnOutEvent(OutEvent payload)
        {
            StartCoroutine(OnOutEventRoutine(payload));
        }

        IEnumerator OnOutEventRoutine(OutEvent payload)
        {
            TimeScaleManager.SlowMotion();
            yield return StartCoroutine(_uiManager.ShowOutNotification(payload));
            TimeScaleManager.SetGameplayTimeScale();
            RespawnGameplayElements(payload.FieldSideData.SideType);
        }
        #endregion

        void RespawnGameplayElements(FieldSideType sideType)
        {
            _playersManager.ResetPlayers();
            _ballManager.ResetBallWithSpin(sideType);
            //_ballManager.ResetBall();
            _goalsManager.SetCollidersEnabled(true);
        }

        public bool MatchDone { get; private set; }
        void ShowEndgame(GoalEvent payload)
        {
            MatchDone = true;
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
                AnalyticsService.Instance.RecordEvent(analyticsEvent); // sends to unity dashboard, ask Rishi
                Debug.Log("sent match ended event to analytics");
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