using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio; 

namespace AudioSystem
{
    [RequireComponent(typeof(AudioSource))]
    public class MusicManager : MonoBehaviour
    {
        [SerializeField] private AudioMixerGroup _musicMixerGroup;
        [System.Serializable]
        public struct MusicClip
        {
            public MusicType Type;
            public AudioClip Clip;
        }

        [SerializeField] List<MusicClip> _soundClips;

        Dictionary<MusicType, AudioClip> _clipMap;
        AudioSource _audioSource;

        void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.outputAudioMixerGroup = _musicMixerGroup;
            _clipMap = new Dictionary<MusicType, AudioClip>();

            foreach (MusicClip sound in _soundClips)
            {
                _clipMap[sound.Type] = sound.Clip;
            }
        }

        void Start()
        {
            ChangeMusic(MusicType.MainMenu);
        }

        public void ChangeMusic(MusicType type)
        {
            if (_clipMap.TryGetValue(type, out AudioClip clip) && clip != null)
            {
                _audioSource.clip = clip;
                _audioSource.Play();
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogWarning($"No clip assigned for {type}");
#endif
            }
        }

        public void StopMusic()
        {
            if ( _audioSource != null && _audioSource.isPlaying)
                _audioSource.Stop();
        }
    }
}