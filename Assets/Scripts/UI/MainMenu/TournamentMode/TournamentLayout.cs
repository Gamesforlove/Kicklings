using System.Collections.Generic;
using System.Linq;
using CommonDataTypes;
using UnityEngine;

namespace UI.MainMenu.TournamentMode
{
    public class TournamentLayout : MonoBehaviour
    {
        [SerializeField] BracketEntry[] _bracketEntries;
        
        Tournament _tournament;
        
        public void Show(Tournament tournament)
        {
            _tournament = tournament;
            PopulateTeamEntries();
            gameObject.SetActive(true);
        }

        void PopulateTeamEntries()
        {
            List<Bracket> brackets = new();

            foreach (Round round in _tournament.Rounds)
            {
                brackets.AddRange(round.Brackets);
            }
            
            for (int i = 0; i < brackets.Count; i++)
            {
                _bracketEntries[i].Initialize(brackets[i]);
            }
        }
    }
}