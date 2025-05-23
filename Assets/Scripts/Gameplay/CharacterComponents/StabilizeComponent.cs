using System.Linq;
using UnityEngine;

namespace Gameplay.CharacterComponents
{
    public class StabilizeComponent : MonoBehaviour
    {
        const float MaxForwardRotation = 100f;
        const float MinBackwardRotation = 260f;
        const float RotationFactor = 10f;
        
        [SerializeField] GroundCheck[] _groundChecks;
        [SerializeField] float _stabilizationFactor;

        Rigidbody2D _rigidBody;
        bool _isRecovering;

        void Awake()
        {
            _rigidBody = GetComponent<Rigidbody2D>();
        }

        void FixedUpdate()
        {
            if (_groundChecks.Any(check => check.IsGrounded))
                StabilizeRotation(_stabilizationFactor);
            
            _rigidBody.angularVelocity = Mathf.Clamp(_rigidBody.angularVelocity, -40f, 40f);
        }

        void StabilizeRotation(float factor)
        {
            float currentRotation = transform.rotation.eulerAngles.z;

            if (IsForwardTilt(currentRotation))
            {
                ApplyStabilizingTorque(-factor * (currentRotation / RotationFactor));
            }
            else if (IsBackwardTilt(currentRotation))
            {
                ApplyStabilizingTorque(factor * ((360 - currentRotation) / RotationFactor));
            }
        }

        bool IsForwardTilt(float rotation) => rotation > 0 && rotation < MaxForwardRotation;
        
        bool IsBackwardTilt(float rotation) => rotation > MinBackwardRotation && rotation < 360;

        void ApplyStabilizingTorque(float torque) => _rigidBody.AddTorque(torque);
    }
}