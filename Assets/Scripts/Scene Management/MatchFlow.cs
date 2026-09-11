using CommonDataTypes;
using EventBusSystem;
using UnityEngine;
using System.Diagnostics;
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
        public static void CreateCampaignMatch(MatchSettings matchSettings, bool isReplayMatch = false)
        {
            DisposeMatch();
            CampaignLevelData levelData = matchSettings.LevelData;
            SceneName levelScene = levelData == null ? SceneName.CampaignGameplay : levelData.LevelGameplayScene;
            Match = new CampaignMatch(matchSettings)
            {
                GoAfterCutScene = levelScene,
                IsReplayMatch = isReplayMatch
            };
            SceneName nextScene = levelData.PreMatchCutScene == SceneName.None ? levelScene : levelData.PreMatchCutScene;
            EventBus<OnLoadScene>.Raise(new OnLoadScene(nextScene));
        }
        public static void ContinueCampaign()
        {
            if (Match is CampaignMatch campaignMatch)
            {
                campaignMatch.ContinueCampaign();
            }
            else
            {
                #if UNITY_EDITOR
                    UnityEngine.Debug.LogError("Current match is not campaign match");
                #endif
            }

        }

        public static Match GetCampaignMatch(MatchSettings matchSettings)
        {
            DisposeMatch();
            Match = new CampaignMatch(matchSettings);
            return Match;
        }

        static public void DisposeMatch() => Match?.Dispose();
    }
}