using CommonDataTypes;
using SaveSystem;
using Scene_Management;
using UI.Customization.Clothing;
using UI.Customization.Countries;
using UnityEngine;

public class CampaignMapController : MonoBehaviour
{
    [SerializeField] private CountryCustomizationController _leftCountryCustomizationController, _rightCountryCustomizationController;
    [SerializeField] private RectTransform _character;
    [SerializeField] private StageScrollController _currentStage;
    [SerializeField] private CampaignLevelData _levelData;
    [SerializeField] private CampaignStructure _campaign;

    private void Start()
    {
        if (!SaveLoadGame.Load())
        {
            #if UNITY_EDITOR
                        Debug.LogError("Can't load saved data");
            #endif
        }
        else
        {
            int playerLevel = SaveLoadGame.LoadedData.PlayerLevel;
            int lastLevel = SaveLoadGame.LoadedData.lastUnlockedLevel;
            int stage = SaveLoadGame.LoadedData.stage;
            _currentStage.EnableLevels(playerLevel, stage, _campaign);
            //_currentStage.MoveCharacterAndScrollToLevel(playerLevel);
            //_currentLevelIndex = playerLevel;
        }
    }

    private void MoveCharacterToNextLevel(int currentLevel)
    {
        _currentStage.MoveCharacterAndScrollToLevel(currentLevel + 1);
    }

    public void StartMatch(CampaignLevelData data)
    {
        CampaignTracker.Instance.ReplayLevel(data);
/*        MatchSettings matchSettings = new MatchSettings.Builder()
            .WithNumberOfPlayers(1)
            .WithSplitControls(true)
            .WithIsCampaignMatch(true)
            .WithLevelData(data)
            .Build();

        MatchFlow.CreateCampaignMatch(matchSettings, isReplayMatch: true);*/
    }
    public void ResumeCampaign()
    {
        if (CampaignTracker.Instance)
        {
            CampaignTracker.Instance.PlayNextlevel();
        }
    }
    public void MoveCharacterToLevel(int levelIndex) 
    {

    }
    public void ScrollToLevel(int levelIndex)
    {

    }
}
