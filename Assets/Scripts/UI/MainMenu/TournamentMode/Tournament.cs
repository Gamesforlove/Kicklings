using System.Collections.Generic;
using CommonDataTypes;
using Gameplay.CharacterComponents.Cpu;
using UnityEngine;

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
            LayoutMode = TournamentModeController.GetLayoutMode() switch
            {
                TournamentLayoutMode.Four => 0,
                TournamentLayoutMode.Eight => 1,
                TournamentLayoutMode.Sixteen => 2
            };
            NumberOfRounds = TournamentModeController.GetLayoutMode() switch
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

        public DifficultyLevel GetDifficultyForRound()
        {
            // Easy 1-3 → centered 2 | Medium 4-6 → centered 5 | Hard 7-9 → centered 8
            float baseLevel;
            if (CurrentRound.IsFirstRound())
                baseLevel = 2f;
            else if (CurrentRound.IsLastRound() && NumberOfRounds > 2)
                baseLevel = 8f;
            else
                baseLevel = 5f;

            // T at 0.35 is neutral (shift=0). The range of shift from T then is -2 to +4 depending on how good the player is.
            float t = GameAndPlayerData.Instance != null ? GameAndPlayerData.Instance.T : 0.35f;
            float shift = (t - 0.35f) * 6f;

            // ±0.8 level of noise — only crosses a major difficulty boundary when T has already pushed close to one.
            float noise = Random.Range(-0.8f, 0.8f);

            int level = Mathf.RoundToInt(Mathf.Clamp(baseLevel + shift + noise, 1f, 10f));
            return level switch
            {
                1  => DifficultyLevel.Easy1,
                2  => DifficultyLevel.Easy2,
                3  => DifficultyLevel.Easy3,
                4  => DifficultyLevel.Medium4,
                5  => DifficultyLevel.Medium5,
                6  => DifficultyLevel.Medium6,
                7  => DifficultyLevel.Hard7,
                8  => DifficultyLevel.Hard8,
                9  => DifficultyLevel.Hard9,
                _  => DifficultyLevel.Default
            };
        }
    }
}