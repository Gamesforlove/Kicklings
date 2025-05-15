using System.Collections;
using CommonDataTypes;
using Scene_Management;
using UI.Customization;
using UI.Customization.Clothing;
using UI.Customization.Countries;
using UI.UiSystem.Core;
using UnityEngine;

namespace UI.MainMenu.TournamentMode
{
    public class TournamentModeController : MonoBehaviour
    {
        public static Tournament Tournament;
        [field:SerializeField] public TeamsData TeamsData { get; private set; }
        public TournamentLayoutMode LayoutMode { get; private set; }
        public TeamsData.TeamData PlayerTeamData { get; private set; }
        
        [SerializeField] UIViewsManager _uiViewsManager;
        [SerializeField] UIView _typeSelectionView, _matchConfigurationView, _layoutView, _backgroundView;
        
        [SerializeField] CharacterCustomizationController _characterCustomizationController;
        [SerializeField] CountryCustomizationController _countryCustomizationController;
        [SerializeField] ScoreToWinController _scoreToWinController;

        IEnumerator Start()
        {
            if (MatchFlow.Match == null) yield break;
            
            if (!MatchFlow.Match.Settings.IsTournamentMatch) yield break;
            
            while (!_uiViewsManager.IsReady)
                yield return null;

            _uiViewsManager.TransitionToView(MatchFlow.Match.IsPlayerWinner ? _layoutView : _typeSelectionView);
            _uiViewsManager.ShowView(_backgroundView);
        }

        public void AssignLayoutMode(TournamentLayoutComponent component)
        {
            LayoutMode = component.LayoutMode;
            _uiViewsManager.TransitionToView(_matchConfigurationView);
        }

        public void StartTournament()
        {
            PlayerTeamData = TeamsData.GetTeamById(_countryCustomizationController.TeamDataIndex);
            Tournament = new Tournament(this);
            _uiViewsManager.TransitionToView(_layoutView);
        }

        public void StartMatch()
        {
            MatchSettings matchSettings = new MatchSettings.Builder()
                .WithLeftShirtIndex(_characterCustomizationController.ShirtIndex)
                .WithLeftShoesIndex(_characterCustomizationController.ShoesIndex)
                .WithLeftCountryImageIndex(_countryCustomizationController.TeamDataIndex)
                .WithGoalsToEndMatch(_scoreToWinController.SelectedGoals)
                .WithIsTournamentMatch(true)
                .Build();
            
            MatchFlow.CreateMatch(matchSettings);
        }
    }
}