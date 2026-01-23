using UI.UiSystem.Core;
using UnityEngine;

namespace UI.Gameplay
{
    public class DebugPopupOpener : MonoBehaviour
    {
        UIViewsManager _uiViewsManager;
        GameplayDebugPopup _popup;
    
        void Awake()
        {
            _uiViewsManager = FindFirstObjectByType<UIViewsManager>();
            _popup = FindFirstObjectByType<GameplayDebugPopup>(FindObjectsInactive.Include);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
                _uiViewsManager.ShowView(_popup);
        }
    }
}
