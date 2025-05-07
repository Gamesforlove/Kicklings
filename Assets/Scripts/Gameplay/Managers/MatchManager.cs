using System.Collections;
using CommonDataTypes;
using EventBusSystem;
using Scene_Management;
using UI.Gameplay;
using UI.UiSystem;
using UI.UiSystem.Core;
using UnityEngine;

namespace Gameplay.Managers
{
    public class MatchManager : MonoBehaviour
    {
        [SerializeField] UIViewsManager _uiViewsManager;
        [SerializeField] MatchWinnerView _matchWinnerView;
        [SerializeField] PlayersManager _playersManager;
        [SerializeField] BallManager _ballManager;
        [SerializeField] ScoreBoard _scoreBoard;
        [SerializeField] GameplayNotifications _gameplayNotifications;

        public void ResetGame()
        {
            _playersManager.ResetPlayers();
            _ballManager.ResetBall();
            _scoreBoard.ResetScore();
        }

        public void EndGame()
        {
            MatchFlow.DisposeMatch();
            EventBus<OnLoadScene>.Raise(new OnLoadScene(SceneName.MainMenu));
        }

        void Start()
        {
            _playersManager.SpawnEntities(MatchFlow.MatchSettings);
            _ballManager.SpawnBall();
            _scoreBoard.ResetScore();
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
            _scoreBoard.ChangeScore(payload.ScoringSideData.SideType);
            StartCoroutine(OnGoalEventRoutine(payload));
        }

        void OnOutEvent(OutEvent payload)
        {
            _gameplayNotifications.ShowOutNotification(payload);
            StartCoroutine(OnOutEventRoutine(payload));
        }

        IEnumerator OnGoalEventRoutine(GoalEvent payload)
        {
            Time.timeScale = .2f;
            yield return StartCoroutine(_gameplayNotifications.ShowGoalNotification(payload));
            Time.timeScale = 1f;
            
            if (_scoreBoard.GetScoreFromSide(payload.ScoringSideData.SideType) >=
                MatchFlow.MatchSettings.GoalsToEndMatch)
            {
                _uiViewsManager.ShowView(_matchWinnerView, payload.ScoringSideData);
                yield break;
            }
            
            RespawnGameplayElements(payload.ScoredSideData.SideType);
        }
    
        IEnumerator OnOutEventRoutine(OutEvent evt)
        {
            yield return new WaitForSeconds(1f);
            RespawnGameplayElements(evt.FieldSideData.SideType);
        }

        void RespawnGameplayElements(FieldSideType sideType)
        {
            _playersManager.ResetPlayers();
            _ballManager.ResetBall(sideType);
        }
    }
}