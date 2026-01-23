using UI.UiSystem.Core;

namespace UI.ButtonsBehaviours
{
    public class BackButtonBehaviour : ButtonBehaviour
    {
        UIViewsManager _uiViewsManager;

        protected override void Awake()
        {
            base.Awake();
            _uiViewsManager = FindFirstObjectByType<UIViewsManager>();
        }

        protected override void OnClick()
        {
            _uiViewsManager.BackToPreviousView();
        }
    }
}
