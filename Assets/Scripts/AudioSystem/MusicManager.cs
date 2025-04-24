using System.Collections.Generic;
using UnityEngine;

namespace AudioSystem
{
    [RequireComponent(typeof(AudioSource))]
    public class MusicManager : MonoBehaviour
    {
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
            _clipMap = new Dictionary<MusicType, AudioClip>();

            foreach (MusicClip sound in _soundClips)
            {
                _clipMap[sound.Type] = sound.Clip;
            }
        }

        //TODO: This should be done by the bootstrapper scene
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
                Debug.LogWarning($"No clip assigned for {type}");
            }
        }
    }
    
    public enum MusicType
    {
        MainMenu,
        Gameplay
    }
}