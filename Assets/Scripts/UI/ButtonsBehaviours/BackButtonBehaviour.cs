using UI.UiSystem.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ButtonsBehaviours
{
    [RequireComponent(typeof(Button))]
    public class BackButtonBehaviour : MonoBehaviour
    {
        UIViewsManager _uiViewsManager;
        Button _button;
        void Awake()
        {
            _uiViewsManager = FindFirstObjectByType<UIViewsManager>();
            _button = GetComponent<Button>();
        }

        void Start()
        {
            _button.onClick.AddListener(() => _uiViewsManager.BackToPreviousView());
        }
    }
}
