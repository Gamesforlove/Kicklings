using System.Collections;
using Scene_Management;
using UI.MainMenu.TournamentMode;
using UI.UiSystem.Core;
using UnityEngine;

namespace UI.MainMenu
{
    public class MainMenuFlow : MonoBehaviour
    {
        [Header( "Dependencies" )]
        [SerializeField] UIViewsManager _uiViewsManager;
        [Header("Main menu views")] 
        [SerializeField] UIView _selectionModeView;
        [SerializeField] UIView _freeModeView;
        [Header( "Tournament mode views" )]
        [SerializeField] UIView _typeSelectionView;
        [SerializeField] UIView _matchConfigurationView;
        [SerializeField] UIView _layoutView;
        
        IEnumerator Start()
        {
            while (!_uiViewsManager.IsReady)
                yield return null;
            
            if (MatchFlow.Match == null || !MatchFlow.Match.Settings.IsTournamentMatch) _uiViewsManager.TransitionToView(_selectionModeView);
            else
            {
                if (MatchFlow.Match.IsPlayerWinner)
                    _uiViewsManager.TransitionToViewWithHistory(_selectionModeView, _layoutView);
                else
                {
                    UIView targetView = MatchFlow.Match.IsPlayAgain ? _typeSelectionView : _selectionModeView;
                    _uiViewsManager.TransitionToViewWithHistory(_selectionModeView, targetView);
                }

            }
        }
    }
}