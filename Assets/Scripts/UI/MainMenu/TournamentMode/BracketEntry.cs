using UnityEngine;

namespace UI.MainMenu.TournamentMode
{
    public class BracketEntry : MonoBehaviour
    {
        [SerializeField] TeamEntry[] _teamEntries;
        [SerializeField] GameObject _visualEffect;

        public void Initialize(Bracket bracketData)
        {
            for (int i = 0; i < bracketData.Participants.Length; i++)
            {
                _teamEntries[i].Initialize(bracketData.Participants[i].TeamData);
            }
            
            if (bracketData.IsPlayerBracket()) _visualEffect.SetActive(true);
        }
    }
}