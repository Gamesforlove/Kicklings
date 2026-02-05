using System;
using System.Collections.Generic;

[Serializable]
public class StorageData
{
    public StorageData()
    {
        abilities = new Dictionary<int, string>();
    }

    public int intdata;
    public string stringdata;
    public Dictionary<int, string> abilities;
    /*
     * unlocked abilities
     * player's current level/stage
     * player's atributes
     * unlocked customization item's
     */
}
