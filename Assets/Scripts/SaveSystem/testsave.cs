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
            data.intData = 27;
            data.stringData = "test string";
            data.abilities.Add(1, "kick");
            data.abilities.Add(2, "dash");
            SaveLoadGame.Save(data);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            StorageData data = SaveLoadGame.Load();
            print($"String data: {data.stringData}, int data: {data.intData}\n " +
                $"Dictionary data:\n1. {data.abilities.Keys.ElementAt(0)}, {data.abilities.Values.ElementAt(0)}\n" +
                $"2. {data.abilities.Keys.ElementAt(1)}, {data.abilities.Values.ElementAt(1)}");
        }
    }
}
