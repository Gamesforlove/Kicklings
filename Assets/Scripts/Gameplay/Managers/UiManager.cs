using System.Collections;
using CommonDataTypes;
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
        [SerializeField] GameObject _tournamentKnockOut, _tournamentFinalWinner, _tournamentRoundWinner;
        [SerializeField] GameplayNotifications _gameplayNotifications;
        [SerializeField] ScoreBoard _scoreBoard;

        [Header("Scoring Challenge")]
        //[SerializeField] ChallengeResultView _challengeResultView;
        [SerializeField] ChallengeTimerView _challengeTimerView;
        [SerializeField] ChallengeScoreBoard _challengeScoreBoard;

        void Start()
        {
            _scoreBoard?.ResetScore();
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
        public void ShowTournamentKnockOutView() => _tournamentKnockOut.SetActive(true);
        public void ShowTournamentFinalWinnerView() => _tournamentFinalWinner.SetActive(true);
        public void ShowTournamentRoundWinnerView() => _tournamentRoundWinner.SetActive(true);

        public IEnumerator ShowGoalNotification(GoalEvent payload)
        {
            yield return StartCoroutine(_gameplayNotifications.ShowGoalNotification(payload));
        }
        
        public IEnumerator ShowOutNotification(OutEvent payload)
        {
            yield return StartCoroutine(_gameplayNotifications.ShowOutNotification(payload));
        }

        #region Scoring Challenge
        // Routes through ChallengeScoreBoard, not ScoreBoard — ScoreBoard is built around
        // two competing teams (left/right text, country flags, MatchFlow.Match.Settings)
        // and doesn't map onto "single score vs. a target."
        public void ChangeChallengeScore(int score, int target)
        {
            _challengeScoreBoard.ChangeScore(score, target);
        }

        public void ResetChallengeScore(int target)
        {
            _challengeScoreBoard.ResetScore(target);
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

        // public void ShowChallengeResult(bool won, int score, int target)
        // {
        //     _uiViewsManager.ShowView(_challengeResultView, new ChallengeResultData(won, score, target));
        // }
        #endregion
    }
}