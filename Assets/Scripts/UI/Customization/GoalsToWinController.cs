using Scene_Management;
using TMPro;
using UnityEngine;

namespace UI.Customization
{
    public class ScoreToWinController : MonoBehaviour
    {
        [SerializeField] int _minScore = 1, _maxScore = 5;
        [SerializeField] TextMeshProUGUI _scoreText;

        int _currentTextValue;

        void Start()
        {
            _currentTextValue = _minScore;
            ChangeScoreText(0);
        }

        public void ChangeScoreText(int delta)
        {
            int rangeSize = _maxScore - _minScore + 1;
            int relativeIndex = (_currentTextValue - _minScore + delta + rangeSize) % rangeSize;
            int newIndex = _minScore + relativeIndex;

            _scoreText.text = newIndex.ToString();
            MatchFlow.SetGoalsToEndMatch(newIndex);
            _currentTextValue = newIndex;
        }
    }
}