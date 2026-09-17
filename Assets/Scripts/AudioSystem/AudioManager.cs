using System;
using CommonDataTypes;
using EventBusSystem;
using UnityEngine;

namespace AudioSystem
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] MusicManager _musicManager;
        [SerializeField] EffectsManager _effectsManager;
        void OnEnable()
        {
            EventBus<OnSceneLoaded>.OnEvent += OnSceneLoaded;
            EventBus<PlayerJumped>.OnEvent += OnPlayerActionPerformed;
        }

        void OnDisable()
        {
            EventBus<OnSceneLoaded>.OnEvent -= OnSceneLoaded;
            EventBus<PlayerJumped>.OnEvent -= OnPlayerActionPerformed;
        }
        
        void OnSceneLoaded(OnSceneLoaded evt)
        {
            switch (evt.EnumValue)
            {
                case SceneName.MainMenu:
                case SceneName.CampaignMap:
                case SceneName.TestCampaignCutSceneBeforeMatch:
                case SceneName.TestCampaignCutSceneLose:
                case SceneName.TestCampaignCutSceneWin:
                    _musicManager.ChangeMusic(MusicType.MainMenu);
                    break;
                case SceneName.Gameplay:
                case SceneName.CampaignGameplay:
                case SceneName.Stage0Scene2TutorialMatch:
                    _musicManager.ChangeMusic(MusicType.Gameplay);
                    break;
                default:
                    //_musicManager.ChangeMusic(MusicType.Gameplay);
                    break;
            }
        }
        
        void OnPlayerActionPerformed(PlayerJumped _)
        {
            _effectsManager.PlayPlayerActionPerformed();
        }
    }
}