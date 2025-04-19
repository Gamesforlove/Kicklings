using System;
using System.Collections;
using CommonDataTypes;
using EventBusSystem;
using Scene_Management;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    [SerializeField] PlayersManager _playersManager;
    [SerializeField] BallManager _ballManager;

    void Start()
    {
        _playersManager.SpawnEntities(MatchFlow.SelectedGameModeData);
    }

    void OnEnable()
    {
        EventBus<GoalEvent>.OnEvent += OnGoalEvent;
        EventBus<OutEvent>.OnEvent += OnOutEvent;
    }
    
    void OnDisable()
    {
        EventBus<GoalEvent>.OnEvent -= OnGoalEvent;
        EventBus<OutEvent>.OnEvent -= OnOutEvent;
    }
    
    void OnGoalEvent(GoalEvent evt) => StartCoroutine(OnGoalEventRoutine(evt));

    void OnOutEvent(OutEvent evt) => StartCoroutine(OnOutEventRoutine(evt));

    IEnumerator OnGoalEventRoutine(GoalEvent evt)
    {
        Time.timeScale = .2f;
        yield return new WaitForSeconds(.3f);
        Time.timeScale = 1f;
        ResetGame(evt.FieldSideData.SideType);
    }
    
    IEnumerator OnOutEventRoutine(OutEvent evt)
    {
        yield return new WaitForSeconds(1f);
        ResetGame(evt.FieldSideData.SideType);
    }

    void ResetGame(FieldSideType sideType)
    {
        _playersManager.ResetPlayers();
        _ballManager.ResetBallPositionOnSide(sideType);
    }
}