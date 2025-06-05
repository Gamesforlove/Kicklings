using UnityEngine;
using UnityEngine.Audio;

public class SoundMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer _audioMixer;

    private void Awake()
    {
        LoadVolumes(); // Cargamos los PlayerPrefs automática al inicializar
    }

    private void LoadVolumes()
    {
        // Cargo los PlayerPrefs con valores por defecto en 1 (máximo volumen)
        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        // Aplica los valores al AudioMixer
        SetMasterVolume(masterVolume);
        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume);
    }

    public void SetMasterVolume(float volume)
    {
        _audioMixer.SetFloat("MasterVolume", ConvertToDecibels(volume));
        PlayerPrefs.SetFloat("MasterVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float volume)
    {
        _audioMixer.SetFloat("MusicVolume", ConvertToDecibels(volume));
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        _audioMixer.SetFloat("SFXVolume", ConvertToDecibels(volume));
        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }

    private float ConvertToDecibels(float volume)
    {
        // Convierte el valor lineal (0-1) a decibeles (logarítmico) // para evitar lo que sucedía que se tiraba en mute el sonido una vez se tocaba el slider
        return Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f;
    }
}