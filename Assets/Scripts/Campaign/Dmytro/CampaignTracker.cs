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
            .WithLevelData(levelData)
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
    public void HandleEndgame(bool IsWinner)
    {
        if (!SaveLoadGame.DataIsLoaded)
        {
            #if UNITY_EDITOR
                        Debug.LogError("Data is not loaded");
            #endif
        }

        if (MatchFlow.Match == null || MatchFlow.Match.IsReplayMatch)
        {
            return;
        }

        campaign.CurrentStage.EndgameBehavior.Invoke(campaign, IsWinner);        
    }
    private void OnApplicationQuit()
    {
        if (SaveLoadGame.DataIsLoaded)
        {
            SaveLoadGame.Save(SaveLoadGame.LoadedData);
        }
    }
}
