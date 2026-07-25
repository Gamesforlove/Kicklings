using System.Collections.Generic;
using System.Collections;
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
            public float Volume;
        }

        [SerializeField] List<MusicClip> _soundClips;

        Dictionary<MusicType, AudioClip> _clipMap;
        AudioSource _audioSource;


        public static MusicManager Instance { get; private set; }
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

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
            ChangeMusic(MusicType.MainMenu, instant: true);
        }

        public void ChangeMusic(MusicType type, bool instant = false)
        {
            MusicClip clip = _soundClips.Find(s => s.Type == type);
            if (clip.Clip == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"No clip assigned for {type}");
#endif
                return;
            }


            float volume = clip.Volume;
            if (instant)
            {
                _audioSource.clip = clip.Clip;
                _audioSource.volume = volume;
                _audioSource.Play();
            }
            else
            {
                StopAllCoroutines();
                StartCoroutine(changeMusicRoutine(clip.Clip, volume));
            }
        }

        public void StopMusic()
        {
            if ( _audioSource != null && _audioSource.isPlaying && !fadingOutMusic)
            {
                StopAllCoroutines();
                StartCoroutine(FadeOutMusic(fadeOutTime));
            }
        }

        const float fadeInTime = 0.5f;
        const float fadeOutTime = 0.7f;
        bool inChangeMusicRoutine = false;
        IEnumerator changeMusicRoutine(AudioClip clip, float volume)
        {
            inChangeMusicRoutine = true;
            yield return StartCoroutine(FadeOutMusic(fadeOutTime));
            _audioSource.clip = clip;
            yield return StartCoroutine(FadeInMusic(fadeInTime, volume));
            inChangeMusicRoutine = false;
        }

        bool fadingInMusic = false;
        IEnumerator FadeOutMusic(float duration)
        {
            fadingOutMusic = true;
            float startVolume = _audioSource.volume;
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                _audioSource.volume = Mathf.Lerp(startVolume, 0, t / duration);
                yield return null;
            }
            _audioSource.Stop();
            fadingOutMusic = false;
        }

        bool fadingOutMusic = false;
        IEnumerator FadeInMusic(float duration, float volume)
        {
            fadingInMusic = true;
            _audioSource.volume = 0f;
            _audioSource.Play();
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                _audioSource.volume = Mathf.Lerp(0, volume, t / duration);
                yield return null;
            }
            _audioSource.volume = volume;
            fadingInMusic = false;
        }
    }
}