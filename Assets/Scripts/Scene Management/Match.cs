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
        
        public TournamentMatch(MatchSettings settings, Tournament tournament) : base(settings)
        {
            _tournament = tournament;
        }

        public override void HandleEndgameUI(MatchManager matchManager, UiManager uiManager, GoalEvent goalEvent)
        {
            IsPlayerWinner = goalEvent.ScoringSideData.SideType == FieldSideType.Left;

            if (!IsPlayerWinner)
                uiManager.ShowTournamentKnockOutView();
            else if (_tournament.CurrentRound.IsLastRound())
                uiManager.ShowTournamentWinnerView();
            else
                matchManager.EndGame();
        }
    }
    
    public class CampaignMatch : Match
    {
        public CampaignMatch(MatchSettings settings) : base(settings)
        {
            // more tbd through iteration
        }

        public override void HandleEndgameUI(MatchManager matchManager, UiManager uiManager, GoalEvent goalEvent)
        {
            IsPlayerWinner = goalEvent.ScoringSideData.SideType == FieldSideType.Left;
            uiManager.ShowMatchWinnerView(goalEvent);
        }
    }
}