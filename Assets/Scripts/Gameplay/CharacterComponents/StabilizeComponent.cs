using System.Linq;
using UnityEngine;

namespace Gameplay.CharacterComponents
{
    public class StabilizeComponent : MonoBehaviour
    {
        const float MaxForwardRotation = 100f;
        const float RotationFactor = 10f;
        
        EntityData _entityData;
        
        [SerializeField] GroundCheck[] _groundChecks;
        [Tooltip("Solid body contact with these layers also supports upright recovery, at any tilt.")]
        [SerializeField] LayerMask _bodySupportLayers;

        Rigidbody2D _rigidBody;
        PlayerActions _playerActions;
        Collider2D[] _solidBodyColliders;
        ContactFilter2D _bodySupportFilter;

        void Awake()
        {
            _rigidBody = GetComponent<Rigidbody2D>();
            _playerActions = GetComponent<PlayerActions>();
            if (_bodySupportLayers.value != 0)
            {
                _bodySupportFilter.SetLayerMask(_bodySupportLayers);
                _bodySupportFilter.useTriggers = false;
                _solidBodyColliders = GetComponentsInChildren<Collider2D>(true)
                    .Where(collider => !collider.isTrigger && collider.attachedRigidbody != null)
                    .ToArray();
            }
        }

        void FixedUpdate()
        {
            bool bodySupported = HasBodySupport();
            if (_entityData != null && (bodySupported || _groundChecks.Any(check => check.IsGrounded)))
            {
                float tilt = Mathf.DeltaAngle(0f, _rigidBody.rotation);
                if (Mathf.Abs(tilt) <= _entityData.StabilizationDeadZoneDegrees)
                    SettleGroundedMotion();
                else if (bodySupported || Mathf.Abs(tilt) < MaxForwardRotation)
                    StabilizeRotation(tilt);
            }
             
            _rigidBody.angularVelocity = Mathf.Clamp(_rigidBody.angularVelocity, -40f, 40f);
        }

        public void SetUp(EntityData entityData)
        {
            _entityData = entityData;
        }

        bool HasBodySupport()
        {
            if (_solidBodyColliders == null) return false;

            foreach (var collider in _solidBodyColliders)
            {
                if (collider != null && collider.isActiveAndEnabled && collider.IsTouching(_bodySupportFilter))
                    return true;
            }
            return false;
        }

        void StabilizeRotation(float tilt)
        {
            float correction = Mathf.Clamp(tilt, -MaxForwardRotation, MaxForwardRotation);
            _rigidBody.AddTorque(-_entityData.StabilizationFactor * (correction / RotationFactor));
        }

        void SettleGroundedMotion()
        {
            if (_entityData.GroundedAngularVelocityDeadZone > 0f &&
                Mathf.Abs(_rigidBody.angularVelocity) <= _entityData.GroundedAngularVelocityDeadZone)
                _rigidBody.angularVelocity = 0f;

            bool kickHopActive = _entityData.MinimumKickHopSpeed > 0f &&
                                 _playerActions != null && _playerActions.IsWithinActionContactWindow;
            // Ground sensors can still overlap during takeoff. Do not settle an intentional hop.
            if (!kickHopActive && _entityData.GroundedLinearVelocityDeadZone > 0f &&
                _rigidBody.linearVelocity.sqrMagnitude <=
                _entityData.GroundedLinearVelocityDeadZone * _entityData.GroundedLinearVelocityDeadZone)
                _rigidBody.linearVelocity = Vector2.zero;
        }

    }
}
