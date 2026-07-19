namespace SaveSystem
{
    public static class SaveLoadGame 
    {
        private const string key = "Save_1";
        private static IStorageService storageService = new JsonToFileStorageService();

        public static void Save(StorageData data)
        {
            storageService.Save(key, data);
        }
        public static StorageData Load()
        {
            return storageService.Load<StorageData>(key);
        }
    }
}
