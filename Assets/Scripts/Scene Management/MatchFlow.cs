using CommonDataTypes;
using EventBusSystem;
using System;
using UI.MainMenu.TournamentMode;

namespace Scene_Management
{
    public static class MatchFlow
    {
        public static Match Match { get; private set; }
        public static void CreateMatch(MatchSettings matchSettings)
        {
            DisposeMatch();
            Match = new FreeMatch(matchSettings);
            EventBus<OnLoadScene>.Raise(new OnLoadScene(SceneName.Gameplay));
        }

        public static void CreateTournamentMatch(MatchSettings matchSettings, Tournament tournament)
        {
            DisposeMatch();
            Match = new TournamentMatch(matchSettings, tournament);
            EventBus<OnLoadScene>.Raise(new OnLoadScene(SceneName.Gameplay));
        }

        public static Match CreateCampaignMatch(MatchSettings matchSettings)
        {
            DisposeMatch();
            Match = new CampaignMatch(matchSettings);
            return Match;
        }


        static void DisposeMatch() => Match?.Dispose();
    }
}