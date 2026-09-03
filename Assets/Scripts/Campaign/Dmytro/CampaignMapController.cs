using CommonDataTypes;
using SaveSystem;
using Scene_Management;
using UI.Customization.Clothing;
using UI.Customization.Countries;
using UnityEngine;

public class CampaignMapController : MonoBehaviour
{
    [SerializeField] private CharacterCustomizationController _leftCharacterCustomizationController, _rightCharacterCustomizationController;
    [SerializeField] private CountryCustomizationController _leftCountryCustomizationController, _rightCountryCustomizationController;
    [SerializeField] private GameObject _player1;
    [SerializeField] private GameObject _player2;
    [SerializeField] private RectTransform _character;
    [SerializeField] private StageScrollController _currentStage;
    [SerializeField] private CampaignLevelData _levelData;

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
            _currentStage.EnableLevels(playerLevel, stage);
            //_currentStage.MoveCharacterAndScrollToLevel(playerLevel);
            //_currentLevelIndex = playerLevel;
        }
    }

    private void MoveCharacterToNextLevel(int currentLevel)
    {
        _currentStage.MoveCharacterAndScrollToLevel(currentLevel + 1);
    }

    public void StartMatch(int numberOfPlayers)
    {
        MatchSettings matchSettings = new MatchSettings.Builder()
            .WithNumberOfPlayers(numberOfPlayers)
            //.WithLeftShirtIndex(_leftCharacterCustomizationController.ShirtIndex)
            //.WithLeftShoesIndex(_leftCharacterCustomizationController.ShoesIndex)
            .WithLeftCountryImageIndex(_leftCountryCustomizationController.TeamDataIndex)
            //.WithLeftSkinToneValue(Random.Range(0f, 1f))
            //.WithRightShirtIndex(_rightCharacterCustomizationController.ShirtIndex)
            //.WithRightShoesIndex(_rightCharacterCustomizationController.ShoesIndex)
            .WithRightCountryImageIndex(_rightCountryCustomizationController.TeamDataIndex)
            //.WithRightSkinToneValue(Random.Range(0f, 1f))
            .WithIsCampaignMatch(true)
            .Build();

        MatchFlow.CreateCampaignMatch(matchSettings, isReplayMatch: true);
    }
    public void StartMatch(int numberOfPlayers, GameObject Opponent1, GameObject Opponent2, SceneName preMatchCutScene, SceneName afterMatchCutScene)
    {
        MatchSettings matchSettings = new MatchSettings.Builder()
            .WithNumberOfPlayers(numberOfPlayers)
            .WithLeftCountryImageIndex(_leftCountryCustomizationController.TeamDataIndex)
            .WithRightCountryImageIndex(_rightCountryCustomizationController.TeamDataIndex)
            .WithIsCampaignMatch(true)
            .WithLevelData(_levelData)
            .Build();

        MatchFlow.CreateCampaignMatch(matchSettings, isReplayMatch: true);
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
