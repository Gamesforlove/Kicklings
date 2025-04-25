using UnityEngine;

public class PersistentObjects : MonoBehaviour
{
    public static PersistentObjects Instance;
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
    }
}