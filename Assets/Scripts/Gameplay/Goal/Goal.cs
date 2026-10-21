using UnityEngine;

namespace Gameplay.Goal
{
    public class Goal : MonoBehaviour
    {
        [SerializeField] Collider2D _collider;
        
        public void SetColliderEnabled(bool value)
        {
            // Goal colliders can be destroyed before tutorial cleanup runs on scene unload.
            if (_collider != null)
                _collider.enabled = value;
        }
    }
}
