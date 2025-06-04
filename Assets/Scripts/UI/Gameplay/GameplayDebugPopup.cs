using Gameplay.CharacterComponents;
using Gameplay.CharacterComponents.Cpu;
using Gameplay.Spawners;
using Scene_Management;
using UI.UiSystem.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Gameplay
{
    public class GameplayDebugPopup : UIView
    {
        [SerializeField] UIView _entitiesDataConfigurationView, _cpuDifficultyDataConfigurationView, _matchSettingsConfigurationView;
        [SerializeField] Button _configureEntitiesButton, _configureCpuDifficultyButton, _configureMatchSettingsButton;

        PlayersSpawner _playersSpawner;
        UIViewsManager _uiViewsManager;
        readonly EntityData[] _entitiesData = new EntityData[2];
        CpuDifficultyPreset.DifficultySettings _cpuDifficultySettings;
        
        
        protected override void Awake()
        {
            base.Awake();
            _uiViewsManager = GetComponentInParent<UIViewsManager>();
            _playersSpawner = FindFirstObjectByType<PlayersSpawner>();
        }

        void Start()
        {
            _entitiesData[0] = _playersSpawner.FielderData;
            _entitiesData[1] = _playersSpawner.GoalkeeperData;
            _cpuDifficultySettings = _playersSpawner.CpuDifficultyPreset.GetSettingsForDifficulty(_playersSpawner.CurrentDifficulty);
            
            _configureEntitiesButton.onClick.AddListener(ConfigureEntitiesData);
            _configureCpuDifficultyButton.onClick.AddListener(ConfigureCpuDifficultyData);
            _configureMatchSettingsButton.onClick.AddListener(ConfigureMatchSettings);
        }

        void ConfigureEntitiesData()
        {
            _uiViewsManager.ShowView(_entitiesDataConfigurationView, _entitiesData);
        }

        void ConfigureCpuDifficultyData()
        {
            _uiViewsManager.ShowView(_cpuDifficultyDataConfigurationView, _cpuDifficultySettings);
        }
        
        void ConfigureMatchSettings()
        {
            _uiViewsManager.ShowView(_matchSettingsConfigurationView, MatchFlow.Match.Settings);
        }
    }
}