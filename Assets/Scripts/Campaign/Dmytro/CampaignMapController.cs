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

    private void Start()
    {
        if (MatchFlow.Match == null || !MatchFlow.Match.Settings.IsCampaignMatch)
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
                _currentStage.EnableLevels(lastLevel);
                _currentStage.MoveCharacterAndScrollToLevel(playerLevel);
                //_currentLevelIndex = playerLevel;
            }
        }
        else
        {
            int playerLevel = SaveLoadGame.LoadedData.PlayerLevel;
            int lastLevel = SaveLoadGame.LoadedData.lastUnlockedLevel;

            _currentStage.EnableLevels(lastLevel);
            _currentStage.InstantMoveCharacterToLevel(playerLevel - 1);
            MoveCharacterToNextLevel(playerLevel - 1);

/*            if (MatchFlow.Match.IsPlayerWinner)
            {

            }*/
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

        MatchFlow.CreateCampaignMatch(matchSettings);
    }
    public void StartMatch(int numberOfPlayers, GameObject Opponent1, GameObject Opponent2, SceneName preMatchCutScene, SceneName afterMatchCutScene)
    {
        if (SaveLoadGame.DataIsLoaded)
        {
            SaveLoadGame.LoadedData.PlayerLevel = _currentStage.CharacterLevel;
            SaveLoadGame.Save(SaveLoadGame.LoadedData);
        }
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
            .WithSpecificPlayers(_player1, _player2, Opponent1, Opponent2)
            .WithPreMatchCutScene(preMatchCutScene)
            .WithAfterMatchCutScene(afterMatchCutScene)
            .Build();

        MatchFlow.CreateCampaignMatch(matchSettings);
    }
    public void MoveCharacterToLevel(int levelIndex) 
    {

    }
    public void ScrollToLevel(int levelIndex)
    {

    }
}
