using CommonDataTypes;

namespace UI.MainMenu.TournamentMode
{
    public class Participant
    {
        public TeamsData.TeamData TeamData;
        public readonly bool IsPlayer;

        public Participant(TeamsData.TeamData teamData, bool isPlayer = false)
        {
            TeamData = teamData;
            IsPlayer = isPlayer;
        }
    }
}