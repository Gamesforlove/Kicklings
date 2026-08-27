using CommonDataTypes;
using EventBusSystem;
using Gameplay.Managers;
using SaveSystem;
using System.Collections.Generic;
using UI.MainMenu.TournamentMode;

namespace Scene_Management
{
    public abstract class Match
    {
        public MatchSettings Settings { get; }
        public bool IsPlayerWinner { get; set; }
        public bool IsPlayAgain { get; set; }
        public bool IsReplayMatch { get; set; }
        public bool IsFinished { get; protected set; } = false;
        public SceneName GoAfterCutScene { get; set; } = SceneName.CampaignGameplay;

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
            IsFinished = true;
            IsPlayerWinner = goalEvent.ScoringSideData.SideType == FieldSideType.Left;

            if (IsReplayMatch)
            {
                EventBus<OnLoadScene>.Raise(new OnLoadScene(SceneName.CampaignMap));
                return;
            }
            // Show Win/Lose Screen?
            // uiManager.ShowMatchWinnerView(goalEvent);
            else
            {
                if (IsPlayerWinner) CampaignTracker.Instance.HandleEndgame(IsPlayerWinner);
                if (Settings.AfterMatchCutScene == SceneName.None)
                {
                    
                    CampaignTracker.Instance.PlayNextlevel();
                }
                else
                {
                    SceneName scene = IsPlayerWinner ? Settings.AfterMatchCutScene : Settings.AfterMatchDefeatCutScene;
                    EventBus<OnLoadScene>.Raise(new OnLoadScene(scene));
                }
            }
        }
    }
}