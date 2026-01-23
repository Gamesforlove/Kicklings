using System.Linq;
using EventBusSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay.CharacterComponents
{
    public class PlayerActions : MonoBehaviour
    {
        EntityData _entityData;
        int _kickingDirectionMultiplier = 1;

        const float jumpCdTime = 1f;
        bool _jumpOnCd;
        CountdownTimer _jumpCdTimer;
        
        [SerializeField] GameObject _kickingLeg;
        [SerializeField] GroundCheck[] _groundChecks;
    
        Rigidbody2D _rigidbody;
        HingeJoint2D _kickingLegJoint;
        JointMotor2D _kickingLegJointMotor;
    
        void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _kickingLegJoint = _kickingLeg.GetComponent<HingeJoint2D>();
            _kickingLegJointMotor = _kickingLegJoint.motor;
            _jumpCdTimer = new CountdownTimer(jumpCdTime);
            _jumpCdTimer.OnTimerStop += () => _jumpOnCd = false;
        }

        void Update()
        {
            _jumpCdTimer.Tick(Time.deltaTime);
        }

        public void SetUp(EntityData entityData)
        {
            _entityData = entityData;
            _kickingDirectionMultiplier = transform.position.x > 0 ? -1 : 1;
        }

        public void OnKick(InputAction.CallbackContext context)
        {
            if (context.performed) OnActionPerformed();
            else if (context.canceled) OnActionCancelled();
        }

        public void OnActionPerformed()
        {
            Kick();
            if (_groundChecks.Any(gc =>  gc.IsGrounded) && !_jumpOnCd)
                Jump();
        }

        public void OnActionCancelled()
        {
            ReturnLeftLegToOriginalPosition();
        }
    
        void Jump()
        {
            _rigidbody.AddForce(transform.up * _entityData.JumpPower);
            EventBus<PlayerJumped>.Raise(new PlayerJumped());
            _jumpOnCd = true;
            _jumpCdTimer.Start();
        }

        void Kick()
        {
            ApplyKickingPower(-1);
        }

        void ReturnLeftLegToOriginalPosition()
        {
            ApplyKickingPower(1);
        }
        
        void ApplyKickingPower(float direction)
        {
            _kickingLegJointMotor.motorSpeed = _entityData.KickingPower * _kickingDirectionMultiplier * direction;
            _kickingLegJoint.motor = _kickingLegJointMotor;
        }

    }
}
