using CommonDataTypes;
using UI.UiSystem.Core;
using UnityEngine;

namespace UI.Gameplay
{
    public class MatchSettingsConfigurationView : UIViewWithData<MatchSettings>
    {
        [SerializeField] Configuration _configuration;
        protected override void OnDataReceived(MatchSettings data)
        {
            base.OnDataReceived(data);
            _configuration.SetUpContent(data);
        }
    }
}