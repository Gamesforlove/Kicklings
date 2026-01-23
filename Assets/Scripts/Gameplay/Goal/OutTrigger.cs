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
            Debug.Log("OnTriggerEnter");
        
            if (other.gameObject.GetComponent<BallScript>() == null) return;
        
            EventBus<OutEvent>.Raise(new OutEvent(_fieldSideData));
        }
    }
}
