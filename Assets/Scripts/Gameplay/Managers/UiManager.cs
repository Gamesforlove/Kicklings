using System.Collections;
using EventBusSystem;
using UI.Gameplay;
using UI.UiSystem;
using UI.UiSystem.Core;
using UnityEngine;

namespace Gameplay.Managers
{
    public class UiManager : MonoBehaviour
    {
        [SerializeField] UIViewsManager _uiViewsManager;
        [SerializeField] MatchWinnerView _matchWinnerView;
        [SerializeField] UIView _tournamentKnockOutView, _tournamentWinnerView;
        [SerializeField] GameplayNotifications _gameplayNotifications;
        [SerializeField] ScoreBoard _scoreBoard;

        void Start()
        {
            _scoreBoard.ResetScore();
        }

        public void ChangeScore(int leftScore, int rightScore)
        {
            _scoreBoard.ChangeScore(leftScore, rightScore);
        }
        
        public void ResetGame()
        {
            _scoreBoard.ResetScore();
        }
        
        public void ShowMatchWinnerView(GoalEvent goalEvent) => _uiViewsManager.ShowView(_matchWinnerView, goalEvent.ScoringSideData);
        public void ShowTournamentKnockOutView() => _uiViewsManager.ShowView(_tournamentKnockOutView);
        public void ShowTournamentWinnerView() => _uiViewsManager.ShowView(_tournamentWinnerView);
        
        public IEnumerator ShowGoalNotification(GoalEvent payload)
        {
            yield return StartCoroutine(_gameplayNotifications.ShowGoalNotification(payload));
        }
        
        public IEnumerator ShowOutNotification(OutEvent payload)
        {
            yield return StartCoroutine(_gameplayNotifications.ShowOutNotification(payload));
        }
    }
}