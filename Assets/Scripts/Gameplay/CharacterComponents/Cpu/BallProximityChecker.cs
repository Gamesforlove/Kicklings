using Gameplay.Managers;
using UnityEngine;

namespace Gameplay.CharacterComponents.Cpu
{
    public class BallProximityChecker : MonoBehaviour
    {
        [SerializeField] Transform[] _proximityTransforms;
        [SerializeField] float _speedNormalization = 8f;
        
        CpuDifficultyPreset.ProximityPoint[] _proximityPoints;

        public void SetUp(CpuDifficultyPreset.ProximityPoint[] proximityPoints)
        {
            _proximityPoints = proximityPoints;
            // Ensure we have the same number of transforms as points
            if (_proximityTransforms.Length != _proximityPoints.Length)
            {
                Debug.LogError("Number of proximity transforms doesn't match the number of proximity points!");
            }
        }


        public bool IsBallWithinRange(Rigidbody2D ballRigidbody)
        {
            if (_proximityPoints == null) return false;
            
            float speedMultiplier = ballRigidbody.linearVelocity.magnitude / _speedNormalization;
            Vector3 ballPosition = ballRigidbody.transform.position;
            bool isWithinRange = false;
            
            for (int i = 0; i < _proximityPoints.Length; i++)
            {
                if (!_proximityTransforms[i]) continue;
                
                float detectionRange = _proximityPoints[i].BaseRadius * speedMultiplier;
                
                if (Vector3.Distance(_proximityTransforms[i].position, ballPosition) <= detectionRange)
                {
                    isWithinRange = true;
                    break;
                }
            }
            
            return isWithinRange;
        }

        void OnDrawGizmos()
        {
            if (!Application.isPlaying || _proximityPoints == null) return;

            BallScript ball = BallManager.Instance?.Ball;
            if (!ball) return;

            float speedMultiplier = ball.Rigidbody.linearVelocity.magnitude / _speedNormalization;

            for (int i = 0; i < _proximityPoints.Length; i++)
            {
                if (_proximityTransforms[i] == null) continue;

                float radius = _proximityPoints[i].BaseRadius * speedMultiplier;
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(_proximityTransforms[i].position, radius);
            }
        }
    }
}