using CommonDataTypes;
using EventBusSystem;
using Gameplay.Managers;
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
        public bool IsTournamentWinner { get; private set; }
        public override void HandleEndgameUI(MatchManager matchManager, UiManager uiManager, GoalEvent goalEvent)
        {
            IsPlayerWinner = goalEvent.ScoringSideData.SideType == FieldSideType.Left;

            if (!IsPlayerWinner)
                uiManager.ShowTournamentKnockOutView();
            else if (_tournament.CurrentRound.IsLastRound())
            {
                uiManager.ShowTournamentWinnerView();
                IsTournamentWinner = true;
            }
            else
                matchManager.EndGame();
        }
    }
}