using UnityEngine;

namespace Gameplay.Managers
{
    public class GoalsManager : MonoBehaviour
    {
        public static GoalsManager Instance { get; private set; }
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

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