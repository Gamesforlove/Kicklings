using CommonDataTypes;
using Scene_Management;
using UI.Customization.Clothing;
using UI.Customization.Countries;
using UnityEngine;

namespace UI.MainMenu.Freemode
{
    public class FreemodeController : MonoBehaviour
    {
        [SerializeField] CharacterCustomizationController _leftCharacterCustomizationController, _rightCharacterCustomizationController;
        [SerializeField] CountryCustomizationController _leftCountryCustomizationController, _rightCountryCustomizationController;
        
        public void StartMatch(int numberOfPlayers)
        {
            MatchSettings matchSettings = new MatchSettings.Builder()
                .WithNumberOfPlayers(numberOfPlayers)
                .WithLeftShirtIndex(_leftCharacterCustomizationController.ShirtIndex)
                .WithLeftShoesIndex(_leftCharacterCustomizationController.ShoesIndex)
                .WithLeftCountryImageIndex(_leftCountryCustomizationController.TeamDataIndex)
                .WithRightShirtIndex(_rightCharacterCustomizationController.ShirtIndex)
                .WithRightShoesIndex(_rightCharacterCustomizationController.ShoesIndex)
                .WithRightCountryImageIndex(_rightCountryCustomizationController.TeamDataIndex)
                .Build();
            
            MatchFlow.CreateMatch(matchSettings);
        }
    }
}
