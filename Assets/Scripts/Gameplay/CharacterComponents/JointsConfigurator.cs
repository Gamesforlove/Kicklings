using System;
using UnityEngine;

namespace Gameplay.CharacterComponents
{
    public class JointsConfigurator : MonoBehaviour
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
        
        JointAngleLimits2D _limits;
        
        public void SetJointsLimits()
        {
            if (gameObject.transform.position.x > 0)
                ChangeValuesForRightSide();
            
            for (int i = 0; i < _joints.Length; i++)
            {
                _limits.max = _jointsConfig.Limits[i].Max;
                _limits.min = _jointsConfig.Limits[i].Min;
                _joints[i].limits = _limits;
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