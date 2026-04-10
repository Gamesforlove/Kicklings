using UI.UiSystem.Core;
using UnityEngine;

namespace UI.Gameplay
{
    public class DebugPopupOpener : MonoBehaviour
    {
        UIViewsManager _uiViewsManager;
        [SerializeField] GameplayDebugPopup _popup;
    
        void Awake()
        {
            _uiViewsManager = UIViewsManager.Instance;
            if (_popup == null)
                _popup = FindFirstObjectByType<GameplayDebugPopup>(FindObjectsInactive.Include);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
                _uiViewsManager.ShowView(_popup);
        }
    }
}
