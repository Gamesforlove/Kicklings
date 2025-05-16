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
        static TournamentConfiguration _tournamentConfiguration;
        [field:SerializeField] public TeamsData TeamsData { get; private set; }
        public TeamsData.TeamData PlayerTeamData { get; private set; }
        
        [SerializeField] UIViewsManager _uiViewsManager;
        [SerializeField] UIView _typeSelectionView, _matchConfigurationView, _layoutView, _backgroundView;
        
        [SerializeField] CharacterCustomizationController _characterCustomizationController;
        [SerializeField] CountryCustomizationController _countryCustomizationController;
        [SerializeField] ScoreToWinController _scoreToWinController;
        
        TournamentLayoutComponent _tournamentLayoutComponent;

        IEnumerator Start()
        {
            if (MatchFlow.Match == null) yield break;
            
            if (!MatchFlow.Match.Settings.IsTournamentMatch) yield break;
            
            while (!_uiViewsManager.IsReady)
                yield return null;

            Tournament.SimulateRound(Tournament.CurrentRound);
            _uiViewsManager.TransitionToView(MatchFlow.Match.IsPlayerWinner ? _layoutView : _typeSelectionView);
            _uiViewsManager.ShowView(_backgroundView);
        }

        public TournamentLayoutMode GetLayoutMode() => _tournamentConfiguration.LayoutMode;

        public void AssignLayoutMode(TournamentLayoutComponent component)
        {
            _tournamentLayoutComponent = component;
            _uiViewsManager.TransitionToView(_matchConfigurationView);
        }

        public void StartTournament()
        {
            _tournamentConfiguration = new TournamentConfiguration
            {
                LayoutMode = _tournamentLayoutComponent.LayoutMode,
                PlayerShirtIndex = _characterCustomizationController.ShirtIndex,
                PlayerShoesIndex = _characterCustomizationController.ShoesIndex,
                GoalsToEndMatch = _scoreToWinController.SelectedGoals
            };

            PlayerTeamData = TeamsData.GetTeamById(_countryCustomizationController.TeamDataIndex);
            Tournament = new Tournament(this);
            _uiViewsManager.TransitionToView(_layoutView);
        }

        public void StartMatch()
        {
            Bracket playerBracket = Tournament.GetPlayerBracket();
            Participant player = playerBracket.Participants[0];
            Participant rival =  playerBracket.Participants[1];
            
            MatchSettings matchSettings = new MatchSettings.Builder()
                .WithLeftShirtIndex(_tournamentConfiguration.PlayerShirtIndex)
                .WithLeftShoesIndex(_tournamentConfiguration.PlayerShoesIndex)
                .WithLeftCountryImageIndex(TeamsData.GetTeamByName(player.TeamData.Name).Id)
                .WithRightCountryImageIndex(TeamsData.GetTeamByName(rival.TeamData.Name).Id)
                .WithGoalsToEndMatch(_tournamentConfiguration.GoalsToEndMatch)
                .WithIsTournamentMatch(true)
                .Build();
            
            MatchFlow.CreateMatch(matchSettings);
        }
    }

    public class TournamentConfiguration
    {
        public TournamentLayoutMode LayoutMode { get; set; }
        public TeamsData.TeamData TeamData { get; set; }
        public int PlayerShirtIndex { get; set; }
        public int PlayerShoesIndex { get; set; }
        public int GoalsToEndMatch { get; set; }
    }
}