using System.Collections.Generic;
using CommonDataTypes;

namespace UI.MainMenu.TournamentMode
{
    public class Tournament
    {
        public int NumberOfRounds;
        public List<TeamsData.TeamData> TeamsData;
        public TeamsData.TeamData PlayerTeamData;
        public TeamsData.TeamData[] GeneratedTeamsData;
        
        TournamentModeController _controller;
        int _currentRound;
        TeamsGenerator _teamsGenerator;
        public Tournament(TournamentModeController controller)
        {
            _controller = controller;
            Initialize();
        }

        void Initialize()
        {
            NumberOfRounds = _controller.LayoutMode switch
            {
                TournamentLayoutMode.Four => 1,
                TournamentLayoutMode.Eight => 2,
                TournamentLayoutMode.Sixteen => 3
            };
            _currentRound = 1;
            TeamsData = new List<TeamsData.TeamData>(_controller.TeamsData.Teams);
            PlayerTeamData = _controller.PlayerTeamData;
            _teamsGenerator = new TeamsGenerator(this);
            GeneratedTeamsData = _teamsGenerator.GenerateTeams();
        }
    }
}