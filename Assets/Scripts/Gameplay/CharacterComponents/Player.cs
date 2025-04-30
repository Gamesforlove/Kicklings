using System;
using EventBusSystem;
using UnityEngine;

public class Player : MonoBehaviour
{
    PlayerActions _playerActions;

    void Awake()
    {
        _playerActions = GetComponent<PlayerActions>();
    }

    void OnEnable()
    {
        EventBus<PlayerActionPerformed>.OnEvent += PerformAction;
        EventBus<PlayerActionCanceled>.OnEvent += CancelAction;
    }

    void OnDisable()
    {
        EventBus<PlayerActionPerformed>.OnEvent -= PerformAction;
    }
    
    void PerformAction(PlayerActionPerformed _) => _playerActions.OnActionPerformed();
    void CancelAction(PlayerActionCanceled _) => _playerActions.OnActionCancelled();
}
