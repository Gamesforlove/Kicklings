using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    public void SetMasterVolume(float volume) => audioMixer.SetFloat("masterVolume", Mathf.Log10(volume) * 20);
    public void SetMusicVolume(float volume) => audioMixer.SetFloat("musicVolume", Mathf.Log10(volume) * 20);
    public void SetSFXVolume(float volume) => audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
}
