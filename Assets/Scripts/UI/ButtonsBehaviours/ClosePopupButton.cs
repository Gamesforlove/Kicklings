using UI.UiSystem.Core;

namespace UI.ButtonsBehaviours
{
    public class ClosePopupButton : ButtonBehaviour
    {
        UIViewsManager _uiViewsManager;
        UIView _view;

        protected override void Awake()
        {
            base.Awake();
            _uiViewsManager = FindFirstObjectByType<UIViewsManager>();
            _view = GetComponentInParent<UIView>();
        }

        protected override void OnClick()
        {
            _uiViewsManager.HideView(_view);
        }
    }
}