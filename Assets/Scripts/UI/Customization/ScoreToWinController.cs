using Scene_Management;
using TMPro;
using UnityEngine;

namespace UI.Customization
{
    public class ScoreToWinController : MonoBehaviour
    {
        public int SelectedGoals {get; private set;}
        
        [SerializeField] int _minScore = 1, _maxScore = 5;
        [SerializeField] TextMeshProUGUI _scoreText;
        
        void Start()
        {
            SelectedGoals = _minScore;
            ChangeScoreText(0);
        }

        public void ChangeScoreText(int delta)
        {
            int rangeSize = _maxScore - _minScore + 1;
            int relativeIndex = (SelectedGoals - _minScore + delta + rangeSize) % rangeSize;
            int newIndex = _minScore + relativeIndex;

            _scoreText.text = newIndex.ToString();
            SelectedGoals = newIndex;
        }
    }
}