using System;
using System.Collections;
using CommonDataTypes;
using EventBusSystem;
using Scene_Management;
using UnityEngine;

namespace Gameplay.Managers
{
    public class MatchManager : MonoBehaviour
    {
        [SerializeField] UiManager _uiManager;
        [SerializeField] PlayersManager _playersManager;
        [SerializeField] BallManager _ballManager;
        [SerializeField] GoalsManager _goalsManager;
        
        Match _match;

        int _leftScore, _rightScore;
        public void ResetGame()
        {
            _leftScore = 0;
            _rightScore = 0;
            _uiManager.ChangeScore(_leftScore, _rightScore);
            _playersManager.ResetPlayers();
            _ballManager.ResetBall();
            _goalsManager.SetCollidersEnabled(true);
            TimeScaleManager.SetGameplayTimeScale();
        }

        public void EndGame()
        {
            TimeScaleManager.SetDefaultTimeScale();
            EventBus<OnLoadScene>.Raise(new OnLoadScene(SceneName.MainMenu));
        }

        void Start()
        {
            _match = MatchFlow.Match;
            _playersManager.SpawnEntities(_match.Settings);
            _ballManager.SpawnBall();
            _goalsManager.SetCollidersEnabled(true);
            _leftScore = 0;
            _rightScore = 0;
            TimeScaleManager.SetGameplayTimeScale();
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
            _ballManager.ResetBall(sideType);
            _goalsManager.SetCollidersEnabled(true);
        }

        void ShowEndgame(GoalEvent payload)
        {
            TimeScaleManager.PauseGame();
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