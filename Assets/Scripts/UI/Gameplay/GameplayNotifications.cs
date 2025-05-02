using System;
using CommonDataTypes;
using EventBusSystem;
using UnityEngine;

public class GameplayNotifications : MonoBehaviour
{
    [SerializeField] private FieldSideData _leftFieldSideData,  _rightFieldSideData;
    [SerializeField] GameplayNotification _goalNotification, _outNotification;

    void OnEnable()
    {
        EventBus<GoalEvent>.OnEvent += OnGoalEvent;
        EventBus<OutEvent>.OnEvent += OnOutEvent;
    }
    
    void OnGoalEvent(GoalEvent evt)
    {
        Color color = evt.FieldSideData.SideType == FieldSideType.Left ? _rightFieldSideData.Color : _leftFieldSideData.Color;
        _goalNotification.ChangeColor(color);
        StartCoroutine(_goalNotification.ShowAndHide());
    }

    void OnOutEvent(OutEvent evt)
    {
        StartCoroutine(_outNotification.ShowAndHide());
    }
}