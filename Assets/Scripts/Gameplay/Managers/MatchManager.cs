using System.Collections;
using CommonDataTypes;
using EventBusSystem;
using Scene_Management;
using UI.Gameplay;
using UnityEngine;

namespace Gameplay.Managers
{
    public class MatchManager : MonoBehaviour
    {
        [SerializeField] PlayersManager _playersManager;
        [SerializeField] BallManager _ballManager;
        [SerializeField] ScoreBoard _scoreBoard;

        public void ResetGame()
        {
            _playersManager.ResetPlayers();
            _ballManager.ResetBall();
            _scoreBoard.ResetScore();
        }

        public void EndGame()
        {
            MatchFlow.DisposeMatch();
            EventBus<OnLoadScene>.Raise(new OnLoadScene(SceneName.MainMenu));
        }

        void Start()
        {
            _playersManager.SpawnEntities(MatchFlow.MatchSettings);
            _ballManager.SpawnBall();
            _scoreBoard.ResetScore();
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
            RespawnGameplayElements(evt.FieldSideData.SideType);
        }
    
        IEnumerator OnOutEventRoutine(OutEvent evt)
        {
            yield return new WaitForSeconds(1f);
            RespawnGameplayElements(evt.FieldSideData.SideType);
        }

        void RespawnGameplayElements(FieldSideType sideType)
        {
            _playersManager.ResetPlayers();
            _ballManager.ResetBall(sideType);
        }
    }
}