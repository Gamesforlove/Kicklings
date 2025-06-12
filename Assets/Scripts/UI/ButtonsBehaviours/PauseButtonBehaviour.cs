using Gameplay.Managers;

namespace UI.ButtonsBehaviours
{
    public class PauseButtonBehaviour : ButtonBehaviour
    {
        protected override void OnClick() => TimeScaleManager.PauseGame();
    }
}