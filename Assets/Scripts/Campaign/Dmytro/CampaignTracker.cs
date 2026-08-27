using CommonDataTypes;
using SaveSystem;
using Scene_Management;
using UnityEngine;

public class CampaignTracker : MonoBehaviour
{
    public static CampaignTracker Instance;
    [SerializeField] private CampaignStructure campaign;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void StartCampaign() 
    {
        if (SaveLoadGame.Load())
        {
            StartMatch(1);
        }
        else
        {
            #if UNITY_EDITOR
                        Debug.LogError("Can't load saved data");
            #endif
            return;
        }
    }
    public void PlayNextlevel()
    {
        StartMatch(1);
    }
    public void ReplayLevel(int stage, int level)
    {

    }
    public void StartMatch(int numberOfPlayers)
    {
        if (SaveLoadGame.DataIsLoaded)
        {
            int playerLevel = SaveLoadGame.LoadedData.PlayerLevel;
            int stage = SaveLoadGame.LoadedData.stage;
            CampaignLevelData levelData = campaign.GetLevelData(stage, playerLevel);

            MatchSettings matchSettings = new MatchSettings.Builder()
            .WithNumberOfPlayers(numberOfPlayers)
            .WithIsCampaignMatch(true)
            .WithSpecificPlayers(levelData.Player1, levelData.Player2, levelData.Opponent1, levelData.Opponent2)
            .WithPreMatchCutScene(levelData.PreMatchCutScene)
            .WithAfterMatchCutScene(levelData.AfterMatchCutScene)
            .WithAfterMatchDefeatCutScene(levelData.AfterMatchDefeatCutScene)
            .Build();

            MatchFlow.CreateCampaignMatch(matchSettings);
        }
        else
        {
            #if UNITY_EDITOR
                Debug.LogError("Can't load saved data");
            #endif
            return;
        }
    }
    public void UpdateAndSaveData()
    {
        if (MatchFlow.Match == null || MatchFlow.Match.IsReplayMatch)
        {
            return;
        }
        if (SaveLoadGame.DataIsLoaded)
        {

        }
    }
    public void IncrementAndSaveData()
    {
        if (MatchFlow.Match == null || MatchFlow.Match.IsReplayMatch)
        {
            return;
        }
        if (SaveLoadGame.DataIsLoaded)
        {
            if (SaveLoadGame.LoadedData.PlayerLevel == campaign.Stages[SaveLoadGame.LoadedData.stage].LevelCount - 1)
            {
                SaveLoadGame.LoadedData.stage++;
                SaveLoadGame.LoadedData.PlayerLevel = 0;
            }
            else
            {
                SaveLoadGame.LoadedData.PlayerLevel ++;
            }
            SaveLoadGame.Save(SaveLoadGame.LoadedData);
        }
    }
    public void HandleEndgame(bool IsWinner)
    {
        IncrementAndSaveData();
    }
    private void OnApplicationQuit()
    {
        if (SaveLoadGame.DataIsLoaded)
        {
            SaveLoadGame.Save(SaveLoadGame.LoadedData);
        }
    }
}
