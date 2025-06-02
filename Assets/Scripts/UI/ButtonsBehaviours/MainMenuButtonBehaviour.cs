using CommonDataTypes;
using EventBusSystem;
using UnityEngine;

namespace UI.ButtonsBehaviours
{
    public class MainMenuButtonBehaviour : MonoBehaviour
    {
        public void OnClick()
        {
            EventBus<OnLoadScene>.Raise(new OnLoadScene(SceneName.MainMenu));
        }
    }
}