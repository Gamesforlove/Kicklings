using UnityEngine;

using CommonDataTypes;

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

        void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public void ConfigurePlayerNet(MatchSettings settings)
        {
            bool playerDefendsLeft = settings != null && (settings.IsCampaignMatch || settings.NumberOfPlayers == 1);
            foreach (Goal.Goal goal in _goals)
            {
                if (goal != null)
                    goal.GetComponentInChildren<Goal.GoalTrigger>(true)?.ConfigurePlayerNet(playerDefendsLeft);
            }
        }

        public void SetCollidersEnabled(bool value)
        {
            foreach (Goal.Goal goal in _goals)
            {
                if (goal != null)
                    goal.SetColliderEnabled(value);
            }
        }   
        
    }
}
