using System;
using System.Linq;
using UnityEngine;
    
public class StabilizeComponent : MonoBehaviour
{
    [SerializeField] GroundCheck[] _groundChecks;
    [SerializeField] float _stabilizeFactor;
    
    Rigidbody2D _rigidbody;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        foreach (var groundCheck in _groundChecks)
        {
            if (groundCheck.IsGrounded)
            {
                Stabilize();
            }
        }
    }
    
    void Stabilize()
    {
        Debug.Log("Stabilize");
        
        float zRotation = transform.rotation.eulerAngles.z;

        if (zRotation > 0 && zRotation < 100)
        {
            _rigidbody.AddTorque(-_stabilizeFactor * (zRotation / 10));
        }
        else if (zRotation > 260 && zRotation < 360)
        {
            _rigidbody.AddTorque(_stabilizeFactor * ((360 - zRotation) / 10));
        }

        _rigidbody.angularVelocity = Mathf.Clamp(_rigidbody.angularVelocity, -40f, 40f);
    }
}