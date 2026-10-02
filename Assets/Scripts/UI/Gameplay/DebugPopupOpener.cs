using UI.UiSystem.Core;
using UnityEngine;

namespace UI.Gameplay
{
    public class DebugPopupOpener : MonoBehaviour
    {
        UIViewsManager _uiViewsManager;
        [SerializeField] GameplayDebugPopup _popup;
        void Start()
        {
            //#if !UNITY_EDITOR
            //    this.enabled = false;
            //    return;
            //#endif
            _uiViewsManager = UIViewsManager.Instance;
            if (_popup == null)
                _popup = FindFirstObjectByType<GameplayDebugPopup>(FindObjectsInactive.Include);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
                ToggleDebugPopup();
        }

        bool debugPopupVisible = false;
        void ToggleDebugPopup()
        {
            if (debugPopupVisible)
            {
                _uiViewsManager.HideView(_popup);
                debugPopupVisible = false;
            }
            else
            {
                _uiViewsManager.ShowView(_popup);
                debugPopupVisible = true;
            }
        }
    }
}
