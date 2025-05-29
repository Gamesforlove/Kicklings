using UnityEngine;

namespace Gameplay.Goal
{
    public class Goal : MonoBehaviour
    {
        [SerializeField] Collider2D _collider;
        
        public void SetColliderEnabled(bool value)
        {
            _collider.enabled = value;
        }
    }
}