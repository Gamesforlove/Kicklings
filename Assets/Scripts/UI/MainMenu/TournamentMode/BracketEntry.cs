using UnityEngine;

namespace UI.MainMenu.TournamentMode
{
    public class BracketEntry : MonoBehaviour
    {
        [SerializeField] protected TeamEntry[] TeamEntries;
        [SerializeField] protected BracketVisualEffect VisualEffect;

        public virtual void Initialize(Bracket bracketData)
        {
            for (int i = 0; i < bracketData.Participants.Length; i++)
            {
                TeamEntries[i].ChangeVisualElements(bracketData.Participants[i].TeamData);
            }
            
            if (bracketData.IsPlayerBracket()) VisualEffect.SetActive(true);
        }
        
        public void Clear()
        {
            foreach (TeamEntry entry in TeamEntries)
            {
                entry.Clear();
            }
        }
    }
}