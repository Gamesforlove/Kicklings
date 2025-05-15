using System.Collections.Generic;
using System.Linq;
using CommonDataTypes;
using UnityEngine;

namespace UI.MainMenu.TournamentMode
{
    public class TournamentLayout : MonoBehaviour
    {
        [SerializeField] TeamEntry[] _teamEntries;
        
        Tournament _tournament;
        
        public void Show(Tournament tournament)
        {
            _tournament = tournament;
            PopulateTeamEntries();
            gameObject.SetActive(true);
        }

        void PopulateTeamEntries()
        {
            List<TeamsData.TeamData> generatedTeams = _tournament.GeneratedTeamsData.ToList();

            foreach (TeamEntry teamEntry in _teamEntries)
            {
                TeamsData.TeamData teamData = generatedTeams[Random.Range(0, generatedTeams.Count)];
                teamEntry.Initialize(teamData);
                generatedTeams.Remove(teamData);
            }
        }
    }
}