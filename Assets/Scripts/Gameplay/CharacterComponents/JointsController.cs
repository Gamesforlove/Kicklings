using System;
using UnityEngine;

namespace Gameplay.CharacterComponents
{
    public class JointsController : MonoBehaviour
    {
        [Serializable]
        public class JointsConfig
        {
            [field: SerializeField] public JointLimits[] Limits { get; private set; }
            
            [Serializable]
            public struct JointLimits
            {
                [SerializeField] string _name;
                [field:SerializeField] public float Min { get; set; }
                [field:SerializeField] public float Max { get; set; }
            }
        }
        
        [SerializeField] JointsConfig _jointsConfig;
        [SerializeField] HingeJoint2D[] _joints;
        [Tooltip("Solid limb colliders that overlap in this rig's side view. They still collide with the ball, field, and other players.")]
        [SerializeField] Collider2D[] _overlappingLimbColliders = Array.Empty<Collider2D>();
        
        [Tooltip("Shoulders on the kid rig that stay motor-free when the character resets.")]
        [SerializeField] HingeJoint2D[] _passiveShoulders = Array.Empty<HingeJoint2D>();

        JointAngleLimits2D _limits;

        void OnEnable()
        {
            // Unity clears per-collider ignores when the rig is deactivated.
            for (int i = 0; i < _overlappingLimbColliders.Length; i++)
            {
                var limb = _overlappingLimbColliders[i];
                if (limb == null) continue;
                for (int j = i + 1; j < _overlappingLimbColliders.Length; j++)
                {
                    var other = _overlappingLimbColliders[j];
                    if (other != null && limb.attachedRigidbody != other.attachedRigidbody)
                        Physics2D.IgnoreCollision(limb, other);
                }
            }
        }
        
        public void SetJointsLimits()
        {
            bool rightSide = transform.position.x > 0f;
            bool mirrorKidLimits = rightSide && _passiveShoulders.Length > 0;
            if (rightSide && !mirrorKidLimits)
                ChangeValuesForRightSide();
            
            for (int i = 0; i < _joints.Length; i++)
            {
                var range = _jointsConfig.Limits[i];
                // Mirror the whole joint range on the opposing kid rig.
                _limits.min = mirrorKidLimits ? -range.Max : range.Min;
                _limits.max = mirrorKidLimits ? -range.Min : range.Max;
                _joints[i].limits = _limits;
            }
        }

        public void ResetJoints()
        {
            JointMotor2D motor = new()
            {
                motorSpeed = 0
            };
            
            foreach (HingeJoint2D joint in _joints)
            {
                if (Array.IndexOf(_passiveShoulders, joint) >= 0)
                    continue;
                joint.motor = motor;
            }
        }
        
        void ChangeValuesForRightSide()
        {
            _jointsConfig.Limits[0].Max *= -1;
            _jointsConfig.Limits[1].Min *= -1;
            _jointsConfig.Limits[2].Min *= -1;
        }
    }
}
