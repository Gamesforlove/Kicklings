using System.Collections.Generic;
using CommonDataTypes;
using UnityEngine;

namespace UI.MainMenu.TournamentMode
{
    public class TeamsGenerator
    {
        Tournament _tournament;

        int _numberOfTeamsToGenerate;
        List<TeamsData.TeamData> _teamsData;
        public TeamsGenerator(Tournament tournament)
        {
            _tournament = tournament;
            _numberOfTeamsToGenerate = _tournament.LayoutMode switch
            {
                0 => 4,
                1 => 8,
                2 => 16,
            };
            _teamsData = _tournament.TeamsData;
        }

        public TeamsData.TeamData[] GenerateTeams()
        {
            TeamsData.TeamData[] generatedTeams = new TeamsData.TeamData[_numberOfTeamsToGenerate];
            
            generatedTeams[0] = _tournament.PlayerTeamData;
            _teamsData.Remove(_tournament.PlayerTeamData);

            for (int i = 1; i < _numberOfTeamsToGenerate; i++)
            {
                int randomIndex = Random.Range(0, _teamsData.Count);
                TeamsData.TeamData teamData = _teamsData[randomIndex];
                generatedTeams[i] = teamData;
                _teamsData.RemoveAt(randomIndex);
            }
            
            return generatedTeams;
        }
    }
}