using EventBusSystem;
using UnityEngine;

namespace Gameplay.CharacterComponents.Player
{
    public class Player : Entity
    {
        [field: SerializeField, Tooltip("Optional placement for the shared control prompt when this prefab has no indicator of its own.")]
        public Transform IndicatorAnchor { get; private set; }
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
            EventBus<PlayerActionCanceled>.OnEvent -= CancelAction;
        }
    
        void PerformAction(PlayerActionPerformed _) => _playerActions.OnActionPerformed();
        void CancelAction(PlayerActionCanceled _) => _playerActions.OnActionCancelled();
    }
}
