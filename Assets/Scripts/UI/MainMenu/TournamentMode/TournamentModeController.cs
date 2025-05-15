using CommonDataTypes;
using Scene_Management;
using UI.Customization;
using UI.Customization.Clothing;
using UI.Customization.Countries;
using UnityEngine;

namespace UI.MainMenu.TournamentMode
{
    public class TournamentModeController : MonoBehaviour
    {
        public static Tournament Tournament;
        [field:SerializeField] public TeamsData TeamsData { get; private set; }
        public TournamentLayoutMode LayoutMode { get; private set; }
        public TeamsData.TeamData PlayerTeamData { get; private set; }
        
        [SerializeField] CharacterCustomizationController _characterCustomizationController;
        [SerializeField] CountryCustomizationController _countryCustomizationController;
        [SerializeField] ScoreToWinController _scoreToWinController;
        
        public void AssignLayoutMode(TournamentLayoutComponent component)
        {
            LayoutMode = component.LayoutMode;
        }

        public void StartTournament()
        {
            PlayerTeamData = TeamsData.GetTeamById(_countryCustomizationController.TeamDataIndex);
            Tournament = new Tournament(this);
        }

        public void StartMatch()
        {
            MatchSettings matchSettings = new MatchSettings.Builder()
                .WithLeftShirtIndex(_characterCustomizationController.ShirtIndex)
                .WithLeftShoesIndex(_characterCustomizationController.ShoesIndex)
                .WithLeftCountryImageIndex(_countryCustomizationController.TeamDataIndex)
                .WithGoalsToEndMatch(_scoreToWinController.SelectedGoals)
                .Build();
            
            MatchFlow.CreateTournamentMatch(matchSettings);
        }
    }
}