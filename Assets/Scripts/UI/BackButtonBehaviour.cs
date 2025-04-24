using CommonDataTypes;
using EventBusSystem;
using UnityEngine;

namespace UI
{
    public class BackButtonBehaviour : MonoBehaviour
    {
        public void OnClick()
        {
            EventBus<OnLoadScene>.Raise(new OnLoadScene(SceneName.MainMenu));
        }
    }
}