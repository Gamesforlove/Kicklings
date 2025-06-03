using System.Collections.Generic;
using CommonDataTypes;

namespace UI.MainMenu.TournamentMode
{
    public class Tournament
    {
        public int LayoutMode {get; private set;}
        public List<TeamsData.TeamData> TeamsData;
        public TeamsData.TeamData PlayerTeamData;
        public List<Participant> Participants;
        public Round CurrentRound;
        public List<Round> Rounds = new();
        public int NumberOfRounds { get; private set; }
        
        TournamentModeController _controller;
        TeamsGenerator _teamsGenerator;
        
        public Tournament(TournamentModeController controller)
        {
            _controller = controller;
            Initialize();
        }

        public void SimulateRound(Round round)
        {
            List<Participant> roundWinners = round.GetWinners();
            Participants = roundWinners;
            GenerateRound(CurrentRound.Id + 1);
        }

        public Bracket GetPlayerBracket()
        {
            return CurrentRound.Brackets.Find(bracket => bracket.IsPlayerBracket());
        }
        
        void Initialize()
        {
            LayoutMode = _controller.GetLayoutMode() switch
            {
                TournamentLayoutMode.Four => 0,
                TournamentLayoutMode.Eight => 1,
                TournamentLayoutMode.Sixteen => 2
            };
            NumberOfRounds = _controller.GetLayoutMode() switch
            {
                TournamentLayoutMode.Four => 2,
                TournamentLayoutMode.Eight => 3,
                TournamentLayoutMode.Sixteen => 4
            };
            TeamsData = new List<TeamsData.TeamData>(_controller.TeamsData.Teams);
            PlayerTeamData = _controller.PlayerTeamData;
            _teamsGenerator = new TeamsGenerator(this);
            Participants = _teamsGenerator.GenerateParticipants();
            CurrentRound = GenerateRound(1);
        }
        
        Round GenerateRound(int id)
        {
            Round round = new(id, this);
            Rounds.Add(round);
            CurrentRound = round;
            return round;
        }
    }
}