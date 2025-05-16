using System.Linq;
using UnityEngine;

namespace UI.MainMenu.TournamentMode
{
    public class Bracket
    {
        public readonly Participant[] Participants;
        
        public Bracket(Participant[] participants)
        {
            Participants = participants;
        }

        public Participant GetWinner()
        {
            return IsPlayerBracket() ? 
                Participants.First(participant => participant.IsPlayer) : Participants[Random.Range(0, Participants.Length)];
        }

        public bool IsPlayerBracket()
        {
            return Participants.Any(participant => participant.IsPlayer);
        }
    }
}