using System.Collections.Generic;
using CommonDataTypes;

namespace UI.MainMenu.TournamentMode
{
    public class Tournament
    {
        public int LayoutMode {get; private set;}
        public List<TeamsData.TeamData> TeamsData;
        public TeamsData.TeamData PlayerTeamData;
        public TeamsData.TeamData[] Participants;
        public Round CurrentRound;
        
        TournamentModeController _controller;
        TeamsGenerator _teamsGenerator;
        List<Round> _rounds = new();
        public Tournament(TournamentModeController controller)
        {
            _controller = controller;
            Initialize();
        }
        
        Round GenerateRound(int id)
        {
            Round round = new(id, this);
            _rounds.Add(round);
            return round;
        }

        void Initialize()
        {
            LayoutMode = _controller.LayoutMode switch
            {
                TournamentLayoutMode.Four => 0,
                TournamentLayoutMode.Eight => 1,
                TournamentLayoutMode.Sixteen => 2
            };
            TeamsData = new List<TeamsData.TeamData>(_controller.TeamsData.Teams);
            PlayerTeamData = _controller.PlayerTeamData;
            _teamsGenerator = new TeamsGenerator(this);
            Participants = _teamsGenerator.GenerateTeams();
            CurrentRound = GenerateRound(1);
        }
    }
}