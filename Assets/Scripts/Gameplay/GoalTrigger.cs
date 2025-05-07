using CommonDataTypes;
using EventBusSystem;
using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    [SerializeField] FieldSideData _scoringSideData;
    [SerializeField] FieldSideData _scoredSideData;

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("OnTriggerEnter");
        
        if (other.gameObject.GetComponent<BallScript>() == null) return;
        
        EventBus<GoalEvent>.Raise(new GoalEvent(_scoringSideData, _scoredSideData));
    }
}