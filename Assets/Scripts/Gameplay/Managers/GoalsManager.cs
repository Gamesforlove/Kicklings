using UnityEngine;

namespace Gameplay.Managers
{
    public class GoalsManager : MonoBehaviour
    {
        [SerializeField] Goal.Goal[] _goals;
        
        public void SetCollidersEnabled(bool value)
        {
            foreach (Goal.Goal goal in _goals)
            {
                goal.SetColliderEnabled(value);
            }
        }   
    }
}