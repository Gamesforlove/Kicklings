using UnityEngine;

namespace Gameplay.CharacterComponents
{
    public class StabilizeComponent : MonoBehaviour
    {
        [SerializeField] GroundCheck[] _groundChecks;
        [SerializeField] float _groundFactor, _airborneFactor;
    
        Rigidbody2D _rigidbody;

        void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        void FixedUpdate()
        {
            foreach (GroundCheck groundCheck in _groundChecks)
            {
                Stabilize(groundCheck.IsGrounded ? _groundFactor : _airborneFactor);
            }
        }
    
        void Stabilize(float factor)
        {
            float zRotation = transform.rotation.eulerAngles.z;

            if (zRotation > 0 && zRotation < 100)
            {
                _rigidbody.AddTorque(-_groundFactor * (zRotation / 10));
            }
            else if (zRotation > 260 && zRotation < 360)
            {
                _rigidbody.AddTorque(_groundFactor * ((360 - zRotation) / 10));
            }

            _rigidbody.angularVelocity = Mathf.Clamp(_rigidbody.angularVelocity, -40f, 40f);
        }
    }
}