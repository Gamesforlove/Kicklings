using System.Collections.Generic;
using System.Linq;
using CommonDataTypes;
using Unity.VisualScripting;
using UnityEngine;

namespace UI.MainMenu.TournamentMode
{
    public class Round
    {
        public int Id;
        public readonly List<Bracket> Brackets = new();

        Tournament _tournament;
        public Round(int id, Tournament tournament)
        {
            Id = id;
            _tournament = tournament;
            GenerateBrackets(_tournament.Participants.Count / 2);
        }

        public List<Participant> GetWinners()
        {
            return Brackets.Select(bracket => bracket.GetWinner()).ToList();
        }

        void GenerateBrackets(int numberOfBrackets)
        {
            List<Participant> participants = new(_tournament.Participants);
            
            for (int i = 0; i < numberOfBrackets; i++)
            {
                Participant[] bracketParticipants = GetBracketParticipants(ref participants);
                Brackets.Add(new Bracket(bracketParticipants, this));
            }
        }

        Participant[] GetBracketParticipants(ref List<Participant> participants)
        {
            Participant[] teamsData = { participants[0], participants[1] };
            participants.Remove(participants[0]);
            participants.Remove(participants[0]);
            return teamsData;
        }

        public bool IsCurrentRound() => Id == _tournament.CurrentRound.Id;
        
        public bool IsLastRound() => Id == _tournament.NumberOfRounds;
    }
}