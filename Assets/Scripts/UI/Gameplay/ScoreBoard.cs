using CommonDataTypes;
using EventBusSystem;
using TMPro;
using UnityEngine;

public class ScoreBoard : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _leftScoreText,  _rightScoreText;
    
    int _leftScore, _rightScore;

    public void ResetScore()
    {
        _leftScore = 0;
        _rightScore = 0;
        
        _leftScoreText.text = _leftScore.ToString();
        _rightScoreText.text = _rightScore.ToString();
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
