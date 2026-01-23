using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuManager : MonoBehaviour
{
    [Header("Audio Mixer References")]
    [SerializeField] private SoundMixerManager _soundMixerManager;

    [Header("UI Sliders")]
    [SerializeField] private Slider _masterVolumeSlider;
    [SerializeField] private Slider _musicVolumeSlider;
    [SerializeField] private Slider _sfxVolumeSlider;

    private void OnEnable()
    {
        // Aplico los valores guardados al AudioMixer cada vez que se abre el panel
        InitializeSliders();
        ApplyCurrentVolumesToMixer();
    }

    private void Start()
    {
        SetupSliderListeners();
    }

    private void InitializeSliders()
    {
        // Cargo los valores de audioMixer y sincroniza sliders
        _masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        _musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        _sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
    }

     private void ApplyCurrentVolumesToMixer()
    {
        // Envío los valores actuales de los sliders al AudioMixer para evitar el error del mute al volver a abrir el panel
        _soundMixerManager.SetMasterVolume(_masterVolumeSlider.value);
        _soundMixerManager.SetMusicVolume(_musicVolumeSlider.value);
        _soundMixerManager.SetSFXVolume(_sfxVolumeSlider.value);
    }

    private void SetupSliderListeners()
    {
        _masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        _musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        _sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    private void SetMasterVolume(float volume)
    {
        _soundMixerManager.SetMasterVolume(volume);
    }

    private void SetMusicVolume(float volume)
    {
        _soundMixerManager.SetMusicVolume(volume);
    }

    private void SetSFXVolume(float volume)
    {
        _soundMixerManager.SetSFXVolume(volume);
    }
}