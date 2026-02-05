using System.Linq;
using UnityEngine;

public class testsave : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            StorageData data= new StorageData();
            data.intdata = 27;
            data.stringdata = "test string";
            data.abilities.Add(1, "kick");
            data.abilities.Add(2, "dash");
            StorageServiceManager.Save(data);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            StorageData data = StorageServiceManager.Load();
            print($"String data: {data.stringdata}, int data: {data.intdata}\n " +
                $"Dictionary data:\n1. {data.abilities.Keys.ElementAt(0)}, {data.abilities.Values.ElementAt(0)}\n" +
                $"2. {data.abilities.Keys.ElementAt(1)}, {data.abilities.Values.ElementAt(1)}");
        }
    }
}
