using SaveSystem;
using UnityEngine;

public class ClearSavedData : MonoBehaviour
{
    public void ClearData()
    {
        StorageData data = new StorageData();

        SaveLoadGame.Save(data);
    }
}
