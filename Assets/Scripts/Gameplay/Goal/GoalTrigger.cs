using CommonDataTypes;
using EventBusSystem;
using UnityEngine;

namespace Gameplay.Goal
{
    public class GoalTrigger : MonoBehaviour
    {
        [SerializeField] FieldSideData _scoringSideData;
        [SerializeField] FieldSideData _scoredSideData;

        void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.GetComponent<BallScript>()) return;
        
            EventBus<GoalEvent>.Raise(new GoalEvent(_scoringSideData, _scoredSideData));
        }
    }
}