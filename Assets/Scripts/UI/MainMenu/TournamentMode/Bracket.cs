using CommonDataTypes;

namespace UI.MainMenu.TournamentMode
{
    public class Bracket
    {
        public TeamsData.TeamData[] TeamsData;
        public bool IsPlayerBracket;
        
        public Bracket(TeamsData.TeamData[] teamData)
        {
            TeamsData = teamData;
        }
    }
}