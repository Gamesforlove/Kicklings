using System;
using System.Collections.Generic;


namespace SaveSystem
{
    [Serializable]
    public class StorageData
    {
        public StorageData()
        {
            abilities = new Dictionary<int, string>();
        }

        // campaign progress tracking
        public int stage;
        public int scene;

        public int intData;
        public string stringData;
        public Dictionary<int, string> abilities;
        /*
         * unlocked abilities
         * player's current level/stage
         * player's atributes
         * unlocked customization item's
         */
    }
}
