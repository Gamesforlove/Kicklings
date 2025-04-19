using System;
using EventBusSystem;
using UnityEngine;

public class Player : MonoBehaviour
{
    PlayerActions _playerActions;
    float _horizontalDirection;

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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            EventBus<PlayerActionPerformed>.Raise();

        if (Input.GetKeyUp(KeyCode.Space))
            EventBus<PlayerActionCanceled>.Raise();
    }
    
    void PerformAction(PlayerActionPerformed _) => _playerActions.OnActionPerformed();
    void CancelAction(PlayerActionCanceled _) => _playerActions.OnActionCancelled();
}
