using Gameplay.CharacterComponents;
using UI.UiSystem.Core;
using UnityEngine;

namespace UI.Gameplay
{
    public class EntitiesConfigurationView : UIViewWithData<EntityData[]>
    {
        [SerializeField] Configuration[] _configurations;
        protected override void OnDataReceived(EntityData[] data)
        {
            base.OnDataReceived(data);

            for (int i = 0; i < _configurations.Length; i++)
            {
                _configurations[i].SetUpContent(data[i]);
            }
        }
    }
}