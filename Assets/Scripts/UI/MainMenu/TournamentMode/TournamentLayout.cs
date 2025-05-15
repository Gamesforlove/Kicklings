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
            /*List<TeamsData.TeamData> generatedTeams = _tournament.Participants.ToList();

            foreach (TeamEntry teamEntry in _teamEntries)
            {
                TeamsData.TeamData teamData = generatedTeams[Random.Range(0, generatedTeams.Count)];
                teamEntry.Initialize(teamData);
                generatedTeams.Remove(teamData);
            }*/

            for (int i = 0; i < _tournament.CurrentRound.Brackets.Count; i++)
            {
                Bracket bracket = _tournament.CurrentRound.Brackets[i];
                _bracketEntries[i].Initialize(bracket);
            }
        }
    }
}