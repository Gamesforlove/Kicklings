using System;
using UI.UiSystem.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ButtonsBehaviours
{
    [RequireComponent(typeof(Button))]
    public class ClosePopupButton : MonoBehaviour
    {
        UIViewsManager _uiViewsManager;
        UIView _view;
        Button _button;

        void Awake()
        {
            _uiViewsManager = FindFirstObjectByType<UIViewsManager>();
            _view = GetComponentInParent<UIView>();
            _button = GetComponent<Button>();
        }

        void Start()
        {
            _button.onClick.AddListener(() => _uiViewsManager.HideView(_view));
        }
    }
}