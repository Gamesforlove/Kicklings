using EventBusSystem;
using UnityEngine;

namespace Gameplay.CharacterComponents.Player
{
    public class Player : Entity
    {
        public PlayerActions _playerActions { get; private set; }

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
