using AudioSystem;
using UnityEngine;

public class PersistentObjects : MonoBehaviour
{
    public static PersistentObjects Instance;
    [field: SerializeField] public SoundMixerManager SoundMixerManager { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }

        if (!SoundMixerManager)
        {
            Debug.LogWarning("PersistentObjects -> SoundMixerManager is null");
        }
    }
}