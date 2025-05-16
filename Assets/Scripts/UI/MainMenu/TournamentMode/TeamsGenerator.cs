using System.Collections.Generic;
using CommonDataTypes;
using UnityEngine;

namespace UI.MainMenu.TournamentMode
{
    public class TeamsGenerator
    {
        readonly Tournament _tournament;

        readonly int _numberOfTeamsToGenerate;
        readonly List<TeamsData.TeamData> _teamsData;
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

        public List<Participant> GenerateParticipants()
        {
            List<Participant> generatedParticipants = new() { new Participant(_tournament.PlayerTeamData, true) };
            _teamsData.Remove(_tournament.PlayerTeamData);

            for (int i = 1; i < _numberOfTeamsToGenerate; i++)
            {
                int randomIndex = Random.Range(0, _teamsData.Count);
                TeamsData.TeamData teamData = _teamsData[randomIndex];
                generatedParticipants.Add(new Participant(teamData));
                _teamsData.RemoveAt(randomIndex);
            }
            
            return generatedParticipants;
        }
    }
}