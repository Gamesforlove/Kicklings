using Gameplay.Managers;

namespace UI.ButtonsBehaviours
{
    public class ResumeButton : ButtonBehaviour
    {
        protected override void OnClick()
        {
            TimeScaleManager.SetGameplayTimeScale();
        }
    }
}