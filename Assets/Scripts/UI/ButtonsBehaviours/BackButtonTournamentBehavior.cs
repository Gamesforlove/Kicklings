using UI.ButtonsBehaviours;
using UI.MainMenu.TournamentMode;
using UI.UiSystem.Core;
using UnityEngine;
public class BackButtonTournamentBehavior : BackButtonBehaviour
{
    [SerializeField] UIView _exitConfirmationView;

    protected override void OnClick()
    {
        if (TournamentModeController.Tournament.CurrentRound.Id != 1)
        {
            _uiViewsManager.TransitionToView(_exitConfirmationView);
            return;
        }

        _uiViewsManager.BackToPreviousView();
    }
}
