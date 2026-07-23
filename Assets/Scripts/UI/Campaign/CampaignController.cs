using CommonDataTypes;
using Scene_Management;
using UI.Customization.Clothing;
using UI.Customization.Countries;
using UnityEngine;

public class CampaignController : MonoBehaviour
{
    [SerializeField] CharacterCustomizationController _leftCharacterCustomizationController, _rightCharacterCustomizationController;
    [SerializeField] CountryCustomizationController _leftCountryCustomizationController, _rightCountryCustomizationController;
    [SerializeField] GameObject Player1;
    [SerializeField] GameObject Player2;

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

        MatchFlow.CreateMatch(matchSettings);
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
            .WithSpecificPlayers(Player1, Player2, Opponent1, Opponent2)
            .Build();

        MatchFlow.CreateMatch(matchSettings);
    }
}
