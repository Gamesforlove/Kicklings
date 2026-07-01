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
            // Base Score -> 0 = Easy, 1 = Medium, 2 = Hard
            float baseScore;
            if (CurrentRound.IsFirstRound())
                baseScore = 0f;
            else if (CurrentRound.IsLastRound() && NumberOfRounds > 2)
                baseScore = 2f;
            else
                baseScore = 1f;

            // T is centered at 0.35. Below 0.35 pulls tiers down, above pushes them up.
            float t = GameAndPlayerData.Instance != null ? GameAndPlayerData.Instance.T : 0.35f; // basically player's skill level, starts at 0.35
            float shift = (t - 0.35f) * 2f;
            float noise = Random.Range(-0.4f, 0.4f);
            float score = Mathf.Clamp(baseScore + shift + noise, 0f, 2f);
            if (score < 0.5f)  return DifficultyLevel.Easy;
            if (score >= 1.5f) return DifficultyLevel.Hard;
            return DifficultyLevel.Medium;
        }
    }
}