using Gameplay.CharacterComponents.Cpu;
using UI.UiSystem.Core;
using UnityEngine;

namespace UI.Gameplay
{
    public class CpuDifficultyConfigurationView : UIViewWithData<CpuDifficultyPreset.DifficultySettings>
    {
        [SerializeField] Configuration _configuration;
        protected override void OnDataReceived(CpuDifficultyPreset.DifficultySettings data)
        {
            base.OnDataReceived(data);
            _configuration.SetUpContent(data);
        }
    }
}