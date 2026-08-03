using CommonDataTypes;
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
    [SerializeField] private int _currentLevelIndex;

    private void Start()
    {

        //enable all unlocked levels

        //move to last saved stage
        //move to last saved level
        //_currentLevelIndex = savedLevelIndex;

        if (MatchFlow.Match == null || !MatchFlow.Match.Settings.IsCampaignMatch) return;

        MoveCharacterToNextLevel();
        //Invoke(nameof(MoveCharacterToNextLevel), .2f);
    }

    private void MoveCharacterToNextLevel()
    {
        _currentStage.MoveCharacterAndScrollToLevel(_currentLevelIndex + 1);
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
    public void StartMatch(int numberOfPlayers, GameObject Opponent1, GameObject Opponent2)
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
            .WithSpecificPlayers(_player1, _player2, Opponent1, Opponent2)
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
