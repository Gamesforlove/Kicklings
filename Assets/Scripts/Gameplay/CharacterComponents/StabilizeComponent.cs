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

        Rigidbody2D _rigidBody;
        PlayerActions _playerActions;
        bool _isRecovering;

        void Awake()
        {
            _rigidBody = GetComponent<Rigidbody2D>();
            _playerActions = GetComponent<PlayerActions>();
        }

        void FixedUpdate()
        {
            if (_entityData != null && _groundChecks.Any(check => check.IsGrounded))
            {
                StabilizeRotation(_entityData.StabilizationFactor);
                SettleGroundedMotion();
            }
             
            _rigidBody.angularVelocity = Mathf.Clamp(_rigidBody.angularVelocity, -40f, 40f);
        }

        public void SetUp(EntityData entityData)
        {
            _entityData = entityData;
        }

        void StabilizeRotation(float factor)
        {
            float signedRotation = Mathf.DeltaAngle(0f, transform.rotation.eulerAngles.z);
            if (Mathf.Abs(signedRotation) <= _entityData.StabilizationDeadZoneDegrees)
                return;

            if (Mathf.Abs(signedRotation) < MaxForwardRotation)
                ApplyStabilizingTorque(-factor * (signedRotation / RotationFactor));
        }

        void SettleGroundedMotion()
        {
            float signedRotation = Mathf.DeltaAngle(0f, transform.rotation.eulerAngles.z);
            if (Mathf.Abs(signedRotation) > _entityData.StabilizationDeadZoneDegrees)
                return;

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

        void ApplyStabilizingTorque(float torque) => _rigidBody.AddTorque(torque);
    }
}
