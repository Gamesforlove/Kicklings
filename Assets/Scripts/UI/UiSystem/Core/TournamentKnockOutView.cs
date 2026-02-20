using Gameplay.Managers;
using Scene_Management;
using UnityEngine;
using UnityEngine.UI;

namespace UI.UiSystem.Core
{
    public class TournamentKnockOutView : UIView
    {
        [SerializeField] Button _playAgainButton, _mainMenuButton;
        
        MatchManager _matchManager;

        protected override void Awake()
        {
            base.Awake();
            _matchManager = FindFirstObjectByType<MatchManager>();
        }
        
        void Start()
        {
            _playAgainButton.onClick.AddListener(OnPlayAgainClicked);
            if (_matchManager)
                _mainMenuButton.onClick.AddListener(_matchManager.EndGame);
        }

        void OnPlayAgainClicked()
        {
            MatchFlow.Match.IsPlayAgain = true;
            _matchManager.EndGame();
        }
    }
}