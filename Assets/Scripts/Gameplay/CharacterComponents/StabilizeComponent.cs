using System.Collections;
using UnityEngine;

namespace Gameplay.CharacterComponents
{
    public class StabilizeComponent : MonoBehaviour
    {
        [SerializeField] GroundCheck[] _groundChecks;
        [SerializeField] float _groundFactor, _airborneFactor;
        [SerializeField] float _sleepVelocityOnBack, _sleepVelocityOnFace;
    
        Rigidbody2D _rigidbody;
        bool _sleeping;

        void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        void FixedUpdate()
        {
            Stabilize(_groundChecks[1].IsGrounded ? _groundFactor : _airborneFactor);
            SleepDetect();
            
            //_rigidbody.angularVelocity = Mathf.Clamp(_rigidbody.angularVelocity, -80f, 80f);
            //Debug.Log(_rigidbody.angularVelocity);
        }
    
        void Stabilize(float factor)
        {
            float zRotation = transform.rotation.eulerAngles.z;
            
            Debug.Log(zRotation);

            if (zRotation > 0 && zRotation < 100)
            {
                _rigidbody.AddTorque(-factor * (zRotation / 10));
            }
            else if (zRotation > 260 && zRotation < 360)
            {
                _rigidbody.AddTorque(factor * ((360 - zRotation) / 10));
            }
        }
        
        void SleepDetect()
        {
            if (_sleeping) return;

            float eulerAnglesZ = transform.rotation.eulerAngles.z;
            
            if (eulerAnglesZ > 60 && eulerAnglesZ < 90)
            {
                StartCoroutine(StandUp(true));
                _sleeping = true;
            }

            else if (eulerAnglesZ > 240 && eulerAnglesZ < 300)
            {
                StartCoroutine(StandUp(false));
                _sleeping = true;
            }
        }
        
        IEnumerator StandUp(bool side)
        {
            yield return new WaitForSeconds(1);
            
            if (side)
            {
                    _rigidbody.angularVelocity = -_sleepVelocityOnBack;
            }
            else
            {
                    _rigidbody.angularVelocity = _sleepVelocityOnFace;
            }

            yield return new WaitForSeconds(2);
            _sleeping = false;
        }
    }
}