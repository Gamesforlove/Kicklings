using UnityEngine;

public class PersistentObjects : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(this);
    }
}