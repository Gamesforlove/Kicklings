using CommonDataTypes;
using EventBusSystem;
using UnityEngine;

namespace Gameplay.Goal
{
    public class OutTrigger : MonoBehaviour
    {
        [SerializeField] FieldSideData _fieldSideData;

        void OnTriggerEnter2D(Collider2D other)
        {
#if UNITY_EDITOR
            Debug.Log("OnTriggerEnter");
#endif
            //if (other.gameObject.GetComponent<BallScript>() == null) return;
            if (other.CompareTag(OutTriggers.Ball.ToString()) || other.CompareTag(OutTriggers.Entity.ToString()))
            {
                EventBus<OutEvent>.Raise(new OutEvent(_fieldSideData));
            }
        
        }
        private enum OutTriggers
        {
            Ball,
            Entity
        }
    }
}
