using EventBusSystem;
using UnityEngine;

namespace Gameplay.CharacterComponents.Player
{
    public class Player : Entity
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
}
