using TMPro;
using UI.Gameplay;
using UnityEngine;

namespace Gameplay.ShotAccuracy
{

    public class ShotAccuracyUi : MonoBehaviour
    {
        [SerializeField] ChallengeScoreBoard _scoreBoard;
        [SerializeField] ChallengeTimerView _timerView;
        [SerializeField] ResultsPanel _resultPanel;

        [Header("Shot Accuracy")]
        [SerializeField] TextMeshProUGUI _shotsText;
        [SerializeField] TextMeshProUGUI _feedbackText;
        [SerializeField] string _shotsFormat = "Shot {0} / {1}";

        [Header("Feedback Thresholds")]
        [SerializeField, Range(0f, 1f)] float _perfectThreshold = 0.95f;
        [SerializeField, Range(0f, 1f)] float _goodThreshold = 0.5f;

        public void InitializeRound(int scoreTarget, float duration, int totalShots)
        {
            _resultPanel?.Hide();
            _scoreBoard?.ResetScore(scoreTarget);
            _timerView?.SetMaxTime(duration);
            _timerView?.ResetView();
            UpdateShots(0, totalShots);
            ClearFeedback();
        }

        public void UpdateScore(int score, int target) => _scoreBoard?.ChangeScore(score, target);

        public void UpdateTimer(float remaining) => _timerView?.SetTime(remaining);

        public void UpdateShots(int used, int total)
        {
            if (_shotsText == null) return;
            _shotsText.text = string.Format(_shotsFormat, Mathf.Min(used + 1, total), total);
        }

        public void ShowShotFeedback(float accuracy, int points)
        {
            if (_feedbackText == null) return;

            if (accuracy >= _perfectThreshold) _feedbackText.text = $"PERFECT!  +{points}";
            else if (accuracy >= _goodThreshold) _feedbackText.text = $"Nice  +{points}";
            else if (points > 0) _feedbackText.text = $"+{points}";
            else _feedbackText.text = "Miss";
        }

        public void ClearFeedback()
        {
            if (_feedbackText != null) _feedbackText.text = string.Empty;
        }

        public void ShowResult(bool won, int score, int target)
        {
            _resultPanel?.Show(won, score, target);
        }
    }
}