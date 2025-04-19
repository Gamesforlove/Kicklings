using System.Collections;
using System.Linq;
using EventBusSystem;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerActions : MonoBehaviour
{
    [SerializeField] float _speed = 8f;
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

    public void OnActionPerformed()
    {
        Kick();
        if (_groundChecks.Any(gc =>  gc.IsGrounded))
            Jump();
    }

    public void OnActionCancelled()
    {
        StartCoroutine(ReturnLeftLegToOriginalPosition());
    }
    
    void Jump()
    {
        _rigidbody.AddForce(Vector2.up * _jumpPower, ForceMode2D.Impulse);
        EventBus<PlayerJumped>.Raise(new PlayerJumped());
    }

    void Kick()
    {
        Debug.Log("Kick");
        _kickingLegJointMotor.motorSpeed = -KickingLegSpeed;
        _kickingLegJoint.motor = _kickingLegJointMotor;
    }

    IEnumerator ReturnLeftLegToOriginalPosition()
    {
        Debug.Log("ReturnLeftLegToOriginalPosition");
        _kickingLegJointMotor.motorSpeed = KickingLegSpeed;
        _kickingLegJoint.motor = _kickingLegJointMotor;
        yield return new WaitForSeconds(0.2f);
        _kickingLegJointMotor.motorSpeed = 0;
        _kickingLegJoint.motor = _kickingLegJointMotor;
    }
}
