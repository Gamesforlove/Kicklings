using SaveSystem;
using Scene_Management;
using UnityEngine;

public class EndgameBehaviour
{
    public static void IncrementAndSaveData(CampaignStructure campaign)
    {
        if (SaveLoadGame.DataIsLoaded)
        {
            if (SaveLoadGame.LoadedData.PlayerLevel == campaign.Stages[SaveLoadGame.LoadedData.stage].LevelCount - 1)
            {
                SaveLoadGame.LoadedData.stage++;
                SaveLoadGame.LoadedData.PlayerLevel = 0;
            }
            else
            {
                SaveLoadGame.LoadedData.PlayerLevel++;
            }
            SaveLoadGame.Save(SaveLoadGame.LoadedData);
        }
        else
        {
            #if UNITY_EDITOR
                Debug.LogError("Data is not loaded");
            #endif
        }
    }
    public static void HandleLinearEndgame(CampaignStructure campaign, bool IsWinner)
    {
        if (IsWinner) IncrementAndSaveData(campaign);
    }
    public static void HandleTournamentEndgame(CampaignStructure campaign, bool IsWinner)
    {
        //IncrementAndSaveData(campaign);
    }
    public static void HandleSeasonEndgame(CampaignStructure campaign, bool IsWinner)
    {
        //IncrementAndSaveData(campaign);
    }
}
