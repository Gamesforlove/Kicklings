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
            _matchManager = MatchManager.Instance;
        }
        
        void Start()
        {
            CustomizeCharacters();
            _playAgainButton.onClick.AddListener(OnPlayAgainClicked);
            if (_matchManager)
                _mainMenuButton.onClick.AddListener(_matchManager.EndGame);
        }

        void OnPlayAgainClicked()
        {
            MatchFlow.Match.IsPlayAgain = true;
            _matchManager.EndGame();
        }
        void CustomizeCharacters()
        {
            _customization1.SetCountryOutfit(_teamsData.GetTeamById(MatchFlow.Match.Settings.LeftCountryImageIndex));
            _customization2.SetCountryOutfit(_teamsData.GetTeamById(MatchFlow.Match.Settings.LeftCountryImageIndex));
        }
    }
}