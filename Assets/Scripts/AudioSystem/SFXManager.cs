using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Audio;

public class SFXManager : MonoBehaviour
{
    [SerializeField] private AudioMixerGroup _sfxMixerGroup;
    public static SFXManager Instance { get; private set; }

    [SerializeField] private AudioSource soundObject;

    public void Awake()
    {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public void PlaySoundEffectAtPoint(AudioClip audioClip, Transform soundTransform, float volume)
    {
        AudioSource audioSource = Instantiate(soundObject, soundTransform.position, Quaternion.identity, transform);
        audioSource.outputAudioMixerGroup = _sfxMixerGroup;
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();

        float clipLength = audioClip.length;
        Destroy(audioSource.gameObject, clipLength);
    }

    public void PlayRandomSoundEffectAtPoint(AudioClip[] audioClips, Transform soundTransform, float volume)
    {
        int randomIndex = Random.Range(0, audioClips.Length);
        AudioClip randomClip = audioClips[randomIndex];

        AudioSource audioSource = Instantiate(soundObject, soundTransform.position, Quaternion.identity, transform);
        audioSource.outputAudioMixerGroup = _sfxMixerGroup;
        audioSource.clip = randomClip;
        audioSource.volume = volume;
        audioSource.Play();

        float clipLength = randomClip.length;
        Destroy(audioSource.gameObject, clipLength);
    }
}
