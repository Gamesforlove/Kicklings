using System;
using UnityEngine;

namespace Gameplay.CharacterComponents
{
    public class BodyPartsController : MonoBehaviour
    {
        [SerializeField] Transform[] _bodyParts;
        
        Rigidbody2D[] _bodyPartsRigidBodies;

        void Awake()
        {
            _bodyPartsRigidBodies = new Rigidbody2D[_bodyParts.Length];
        }

        void Start()
        {
            for (int i = 0; i < _bodyParts.Length; i++)
            {
                _bodyPartsRigidBodies[i] = _bodyParts[i].GetComponent<Rigidbody2D>();
            }
        }

        public void ResetBodyParts()
        {
            for (int i = 0; i < _bodyParts.Length; i++)
            {
                _bodyParts[i].localEulerAngles = Vector3.zero;
                _bodyPartsRigidBodies[i].linearVelocity = Vector3.zero;
                _bodyPartsRigidBodies[i].angularVelocity = 0;
            }
        }
    }
}