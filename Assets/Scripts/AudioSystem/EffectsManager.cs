using System;
using UnityEngine;
using UnityEngine.Audio;

namespace AudioSystem
{
    [RequireComponent(typeof(AudioSource))]
    public class EffectsManager : MonoBehaviour
    {
        [SerializeField] AudioMixerGroup _sfxMixerGroup;
        [SerializeField] AudioClip _playerActionPerformedClip;

        AudioSource _audioSource;

        void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            
        }

        void Start()
        {
            _audioSource.outputAudioMixerGroup = _sfxMixerGroup;
        }

        public void PlayPlayerActionPerformed() => PlaySound(_playerActionPerformedClip);

        void PlaySound(AudioClip clip)
        {
            if (clip != null)
                _audioSource.PlayOneShot(clip);
        }
    }
}