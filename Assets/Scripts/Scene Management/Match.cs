using CommonDataTypes;
using EventBusSystem;
using Gameplay.Managers;
using System.Collections.Generic;
using UI.MainMenu.TournamentMode;

namespace Scene_Management
{
    public abstract class Match
    {
        public MatchSettings Settings { get; }
        public bool IsPlayerWinner { get; set; }
        public bool IsPlayAgain { get; set; }

        protected Match(MatchSettings settings)
        {
            Settings = settings;
        }

        public void Dispose()
        {
            Settings.Dispose();
            IsPlayerWinner = false;
            IsPlayAgain = false;
        }
        
        public abstract void HandleEndgameUI(MatchManager matchManager, UiManager uiManager, GoalEvent goalEvent);
    }

    public class FreeMatch : Match
    {
        public FreeMatch(MatchSettings settings) : base(settings) { }
        
        public override void HandleEndgameUI(MatchManager matchManager, UiManager uiManager, GoalEvent goalEvent)
        {
            IsPlayerWinner = goalEvent.ScoringSideData.SideType == FieldSideType.Left;
            uiManager.ShowMatchWinnerView(goalEvent);   
        }

    }
    public class TournamentMatch : Match
    {
        readonly Tournament _tournament;
        public Tournament Tournament => _tournament;

        public TournamentMatch(MatchSettings settings, Tournament tournament) : base(settings)
        {
            _tournament = tournament;
        }
        public bool IsTournamentWinner { get; private set; }
        public override void HandleEndgameUI(MatchManager matchManager, UiManager uiManager, GoalEvent goalEvent)
        {
            IsPlayerWinner = goalEvent.ScoringSideData.SideType == FieldSideType.Left;

            var data = GameAndPlayerData.Instance;
            if (data != null)
            {
                data.numTournamentMatchesPlayed++;
                data.numTournamentMatchesPlayedToday++;

                if (IsPlayerWinner)
                {
                    data.numTournamentMatchesWon++;
                    data.numTournamentMatchesWonToday++;
                }
                else
                {
                    data.numTournamentMatchesLost++;
                    data.numTournamentMatchesLostToday++;
                }
            }

            if (!IsPlayerWinner)
                uiManager.ShowTournamentKnockOutView();
            else if (_tournament.CurrentRound.IsLastRound())
            {
                uiManager.ShowTournamentFinalWinnerView();
                IsTournamentWinner = true;
            }
            else
            {
                uiManager.ShowTournamentRoundWinnerView();
            }
        }
    }
    
    public class CampaignMatch : Match
    {
        public CampaignMatch(MatchSettings settings) : base(settings) { }

        public override void HandleEndgameUI(MatchManager matchManager, UiManager uiManager, GoalEvent goalEvent)
        {
            IsPlayerWinner = goalEvent.ScoringSideData.SideType == FieldSideType.Left;
            //ReturnToCampaignMap();
            uiManager.ShowMatchWinnerView(goalEvent);
        }
        private void ReturnToCampaignMap()
        {
            SceneHandler.LoadScene(SceneName.CampaignMap);
        }
    }
}