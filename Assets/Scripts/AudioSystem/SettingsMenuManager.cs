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

    private void Start()
    {
        // Cargo los valores guardados al iniciar
        LoadVolumeSettings();
        
        // Configuro listeners para los sliders
        _masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        _musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        _sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    private void SetMasterVolume(float volume)
    {
        _soundMixerManager.SetMasterVolume(volume);
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    private void SetMusicVolume(float volume)
    {
        _soundMixerManager.SetMusicVolume(volume);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    private void SetSFXVolume(float volume)
    {
        _soundMixerManager.SetSFXVolume(volume);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    private void LoadVolumeSettings()
    {
        // Cargo los valores guardados o uso 1 (de volumen máximo) si no existen
        _masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        _musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        _sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
    }
}