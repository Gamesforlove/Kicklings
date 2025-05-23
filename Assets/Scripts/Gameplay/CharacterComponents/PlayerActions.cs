using System;
using System.Collections;
using System.Linq;
using EventBusSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay.CharacterComponents
{
    public class PlayerActions : MonoBehaviour
    {
        [SerializeField] float _jumpPower = 10f;
        [SerializeField] GameObject _kickingLeg;
        [SerializeField] GroundCheck[] _groundChecks;

        public float KickingLegSpeed = 800f;
    
        Rigidbody2D _rigidbody;
        HingeJoint2D _kickingLegJoint;
        JointMotor2D _kickingLegJointMotor;
    
        void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _kickingLegJoint = _kickingLeg.GetComponent<HingeJoint2D>();
            _kickingLegJointMotor = _kickingLegJoint.motor;
        }

        public void OnKick(InputAction.CallbackContext context)
        {
            if (context.performed) OnActionPerformed();
            else if (context.canceled) OnActionCancelled();
        }

        public void OnActionPerformed()
        {
            Kick();
            if (_groundChecks.Any(gc =>  gc.IsGrounded))
                Jump();
        }

        public void OnActionCancelled()
        {
            ReturnLeftLegToOriginalPosition();
        }
    
        void Jump()
        {
            _rigidbody.AddForce(new Vector2(transform.up.x, Mathf.Abs(transform.up.y)) * _jumpPower);
            EventBus<PlayerJumped>.Raise(new PlayerJumped());
        }

        void Kick()
        {
            _kickingLegJointMotor.motorSpeed = -KickingLegSpeed;
            _kickingLegJoint.motor = _kickingLegJointMotor;
        }

        void ReturnLeftLegToOriginalPosition()
        {
            _kickingLegJointMotor.motorSpeed = KickingLegSpeed;
            _kickingLegJoint.motor = _kickingLegJointMotor;
        }
    }
}
