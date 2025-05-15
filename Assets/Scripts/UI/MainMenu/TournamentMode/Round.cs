using System.Collections.Generic;
using System.Linq;
using CommonDataTypes;
using UnityEngine;

namespace UI.MainMenu.TournamentMode
{
    public class Round
    {
        public int Id;
        public List<Bracket> Brackets = new();

        Tournament _tournament;
        public Round(int id, Tournament tournament)
        {
            Id = id;
            _tournament = tournament;
            GenerateBrackets(_tournament.Participants.Length / 2);
        }

        void GenerateBrackets(int numberOfBrackets)
        {
            List<TeamsData.TeamData> participants = new(_tournament.Participants);
            
            for (int i = 0; i < numberOfBrackets; i++)
            {
                TeamsData.TeamData[] teamsData = { GetRandomTeam(ref participants), GetRandomTeam(ref participants) };
                Bracket bracket = new(teamsData);
                if (teamsData.Any(data => data == _tournament.PlayerTeamData)) bracket.IsPlayerBracket = true;
                Brackets.Add(new Bracket(teamsData));
            }
        }

        TeamsData.TeamData GetRandomTeam(ref List<TeamsData.TeamData> participants)
        {
            TeamsData.TeamData randomParticipant = participants[Random.Range(0, participants.Count)];
            participants.Remove(randomParticipant);
            return randomParticipant;
        }
    }
}