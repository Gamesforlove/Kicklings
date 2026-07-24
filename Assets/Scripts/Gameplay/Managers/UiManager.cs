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

       
        [SerializeField] UIView _challengeResultView;     
        [SerializeField] ChallengeTimerView _challengeTimerView;

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

        #region Scoring Challenge
        // Dedicated challenge-mode score display — distinct from ChangeScore(left, right),
        // which is built for two-sided matches and would mislabel score/target.
        public void ChangeChallengeScore(int score, int target)
        {
            _scoreBoard.ChangeScore(score, target);
        }

        public void InitializeChallengeTimer(float maxTime)
        {
            _challengeTimerView?.SetMaxTime(maxTime);
            _challengeTimerView?.ResetView();
        }

        public void UpdateTimer(float timeRemaining)
        {
            _challengeTimerView?.SetTime(timeRemaining);
        }

        public void ShowChallengeResult(bool won, int score, int target)
        {
            _uiViewsManager.ShowView(_challengeResultView, new ChallengeResultData(won, score, target));
        }
        #endregion
    }
    
    public readonly struct ChallengeResultData
    {
        public readonly bool Won;
        public readonly int Score;
        public readonly int Target;

        public ChallengeResultData(bool won, int score, int target)
        {
            Won = won;
            Score = score;
            Target = target;
        }
    }
}