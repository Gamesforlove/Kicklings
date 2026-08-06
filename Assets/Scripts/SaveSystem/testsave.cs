using System.Linq;
using UnityEngine;
using SaveSystem;

public class testsave : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            StorageData data= new StorageData();
            data.abilities.Add(1, "kick");
            data.abilities.Add(2, "dash");
            SaveLoadGame.Save(data);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (SaveLoadGame.Load())
            {
                print(SaveLoadGame.LoadedData);
            }            
        }
    }
}
