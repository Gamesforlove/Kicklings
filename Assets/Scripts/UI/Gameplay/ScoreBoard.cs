using CommonDataTypes;
using EventBusSystem;
using Scene_Management;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Gameplay
{
    public class ScoreBoard : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _leftScoreText,  _rightScoreText;
        [SerializeField] Image _leftCountryImage, _rightCountryImage;
        
        int _leftScore, _rightScore;

        public void ResetScore()
        {
            _leftScore = 0;
            _rightScore = 0;
        
            _leftScoreText.text = _leftScore.ToString();
            _rightScoreText.text = _rightScore.ToString();
        }

        void Start()
        {
            _leftCountryImage.sprite = MatchFlow.MatchSettings.LeftCountryImage.sprite;
            _rightCountryImage.sprite = MatchFlow.MatchSettings.RightCountryImage.sprite;
        }

        void OnEnable()
        {
            EventBus<GoalEvent>.OnEvent += OnGoalScored;
        }

        void OnDisable()
        {
            EventBus<GoalEvent>.OnEvent -= OnGoalScored;
        }

        void OnGoalScored(GoalEvent evt)
        {
            if (evt.FieldSideData.SideType == FieldSideType.Right)
            {
                _leftScore++;
                _leftScoreText.text = _leftScore.ToString();
            }
            else if (evt.FieldSideData.SideType == FieldSideType.Left)
            {
                _rightScore++;
                _rightScoreText.text = _rightScore.ToString();
            }
        }
    }
}
