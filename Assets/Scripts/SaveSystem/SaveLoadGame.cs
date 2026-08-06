namespace SaveSystem
{
    public static class SaveLoadGame 
    {
        public static StorageData LoadedData { get; private set; }
        public static bool DataIsLoaded { get { return LoadedData != null; } }

        private const string key = "Save_1";
        private static IStorageService storageService = new JsonToFileStorageService();

        public static void Save(StorageData data)
        {
            storageService.Save(key, data);
        }
        public static bool Load()
        {
            LoadedData = storageService.Load<StorageData>(key);
            return LoadedData != null;
        }
    }
}
