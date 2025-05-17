using CommonDataTypes;
using Scene_Management;
using UI.Customization;
using UI.Customization.Clothing;
using UI.Customization.Countries;
using UnityEngine;

namespace UI.MainMenu.Freemode
{
    public class FreemodeController : MonoBehaviour
    {
        [SerializeField] CharacterCustomizationController _leftCharacterCustomizationController, _rightCharacterCustomizationController;
        [SerializeField] CountryCustomizationController _leftCountryCustomizationController, _rightCountryCustomizationController;
        [SerializeField] ScoreToWinController _scoreToWinController;
        
        public void StartMatch(int goalsToEndMatch)
        {
            MatchSettings matchSettings = new MatchSettings.Builder()
                .WithLeftShirtIndex(_leftCharacterCustomizationController.ShirtIndex)
                .WithLeftShoesIndex(_leftCharacterCustomizationController.ShoesIndex)
                .WithLeftCountryImageIndex(_leftCountryCustomizationController.TeamDataIndex)
                .WithRightShirtIndex(_rightCharacterCustomizationController.ShirtIndex)
                .WithRightShoesIndex(_rightCharacterCustomizationController.ShoesIndex)
                .WithRightCountryImageIndex(_rightCountryCustomizationController.TeamDataIndex)
                .WithGoalsToEndMatch(goalsToEndMatch)
                .Build();
            
            MatchFlow.CreateMatch(matchSettings);
        }
    }
}
