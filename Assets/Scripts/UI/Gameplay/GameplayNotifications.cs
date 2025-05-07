using System.Collections;
using CommonDataTypes;
using EventBusSystem;
using UnityEngine;

namespace UI.Gameplay
{
    public class GameplayNotifications : MonoBehaviour
    {
        [SerializeField] FieldSideData _leftFieldSideData,  _rightFieldSideData;
        [SerializeField] GameplayNotification _goalNotification, _outNotification;
    
        public IEnumerator ShowGoalNotification(GoalEvent payload)
        {
            Color color = payload.ScoringSideData.SideType == FieldSideType.Right ? _rightFieldSideData.Color : _leftFieldSideData.Color;
            _goalNotification.ChangeColor(color);
            yield return StartCoroutine(_goalNotification.ShowAndHide());
        }

        public void ShowOutNotification(OutEvent _)
        {
            StartCoroutine(_outNotification.ShowAndHide());
        }
    }
}