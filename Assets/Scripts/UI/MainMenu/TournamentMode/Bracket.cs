using System.Linq;
using UnityEngine;

namespace UI.MainMenu.TournamentMode
{
    public class Bracket
    {
        public readonly Participant[] Participants;
        
        Round _round;
        
        public Bracket(Participant[] participants, Round round)
        {
            Participants = participants;
            _round = round;
        }

        public Participant GetWinner()
        {
            return IsPlayerBracket() ? 
                Participants.First(participant => participant.IsPlayer) : Participants[Random.Range(0, Participants.Length)];
        }

        public bool IsPlayerBracket()
        {
            return _round.IsCurrentRound() && Participants.Any(participant => participant.IsPlayer);
        }
    }
}