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
        [SerializeField] UIView _tournamentKnockOutView;
        [SerializeField] PlayersManager _playersManager;
        [SerializeField] BallManager _ballManager;
        [SerializeField] ScoreBoard _scoreBoard;
        [SerializeField] GameplayNotifications _gameplayNotifications;
        [SerializeField] GoalsManager _goalsManager;
        
        Match _match;

        public void ResetGame()
        {
            _playersManager.ResetPlayers();
            _ballManager.ResetBall();
            _scoreBoard.ResetScore();
            _goalsManager.SetCollidersEnabled(true);
        }

        public void EndGame()
        {
            Time.timeScale = 1f;
            EventBus<OnLoadScene>.Raise(new OnLoadScene(SceneName.MainMenu));
        }

        void Start()
        {
            _match = MatchFlow.Match;
            _playersManager.SpawnEntities(_match.Settings);
            _ballManager.SpawnBall();
            _scoreBoard.ResetScore();
            _goalsManager.SetCollidersEnabled(true);
            Time.timeScale = 1.5f;
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
            _goalsManager.SetCollidersEnabled(false);
            StartCoroutine(OnGoalEventRoutine(payload));
        }

        void OnOutEvent(OutEvent payload)
        {
            StartCoroutine(OnOutEventRoutine(payload));
        }

        IEnumerator OnGoalEventRoutine(GoalEvent payload)
        {
            Time.timeScale = .2f;
            yield return StartCoroutine(_gameplayNotifications.ShowGoalNotification(payload));
            Time.timeScale = 1.5f;
            
            if (_scoreBoard.GetScoreFromSide(payload.ScoringSideData.SideType) >=
                _match.Settings.GoalsToEndMatch)
            {
                ShowEndgame(payload);
                yield break;
            }
            
            RespawnGameplayElements(payload.ScoredSideData.SideType);
        }
    
        IEnumerator OnOutEventRoutine(OutEvent payload)
        {
            Time.timeScale = .2f;
            yield return StartCoroutine(_gameplayNotifications.ShowOutNotification(payload));
            Time.timeScale = 1.5f;
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
            if(payload.ScoringSideData.SideType == FieldSideType.Left) _match.IsPlayerWinner = true;

            if (_match.Settings.IsTournamentMatch)
            {
                if (!_match.IsPlayerWinner)
                    _uiViewsManager.ShowView(_tournamentKnockOutView);
                else
                    EndGame();
            }
            else
                _uiViewsManager.ShowView(_matchWinnerView, payload.ScoringSideData);
        }
    }
}