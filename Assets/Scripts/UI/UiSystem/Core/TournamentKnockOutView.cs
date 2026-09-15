using CommonDataTypes;
using Gameplay.Managers;
using Scene_Management;
using UI.Customization.Clothing;
using UnityEngine;
using UnityEngine.UI;

namespace UI.UiSystem.Core
{
    public class TournamentKnockOutView : UIView
    {
        [SerializeField] Button _playAgainButton, _mainMenuButton;

        MatchManager _matchManager;

        [Header("Customization")]
        [SerializeField] CharacterCustomizationController _customization1;
        [SerializeField] CharacterCustomizationController _customization2;
        [SerializeField] TeamsData _teamsData;

        protected override void Awake()
        {
            base.Awake();
            _matchManager = FindFirstObjectByType<MatchManager>();
        }

        void Start()
        {
            CustomizeCharacters();
            _playAgainButton.onClick.AddListener(OnPlayAgainClicked);
            _mainMenuButton.onClick.AddListener(OnMainMenuClicked);
        }

        void OnPlayAgainClicked()
        {
            MatchFlow.Match.IsPlayAgain = true;
            _matchManager.EndGame(true);
        }

        void OnMainMenuClicked()
        {
            _matchManager.EndGame(false);
        }

        void CustomizeCharacters()
        {
            var leftTeam = _teamsData.GetTeamById(MatchFlow.Match.Settings.LeftCountryImageIndex);
            _customization1.SetCountryOutfit(leftTeam);
            _customization2.SetCountryOutfit(leftTeam);
        }
    }
}