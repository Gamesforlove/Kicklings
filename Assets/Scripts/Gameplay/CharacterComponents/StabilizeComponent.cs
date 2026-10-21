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
        bool _isRecovering;

        void Awake()
        {
            _rigidBody = GetComponent<Rigidbody2D>();
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

            if (_entityData.GroundedLinearVelocityDeadZone > 0f &&
                _rigidBody.linearVelocity.sqrMagnitude <=
                _entityData.GroundedLinearVelocityDeadZone * _entityData.GroundedLinearVelocityDeadZone)
                _rigidBody.linearVelocity = Vector2.zero;
        }

        void ApplyStabilizingTorque(float torque) => _rigidBody.AddTorque(torque);
    }
}
