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
            EventBus<PlayerJumped>.OnEvent += OnPlayerActionPerformed;
        }

        void OnDisable()
        {
            EventBus<PlayerJumped>.OnEvent -= OnPlayerActionPerformed;
        }

        void OnPlayerActionPerformed(PlayerJumped _)
        {
            _effectsManager.PlayPlayerActionPerformed();
        }
    }
}