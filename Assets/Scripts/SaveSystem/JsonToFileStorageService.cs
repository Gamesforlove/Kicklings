using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

namespace SaveSystem
{
    public class JsonToFileStorageService : IStorageService
    {
        public void Save(string key, object data, Action<bool> callback = null)
        {
            try
            {
                string path = BuildPath(key);
                JsonSerializerSettings settings = MakeSerializerSettings();
                string json = JsonConvert.SerializeObject(data, settings);

                using (var fileStream = new StreamWriter(path))
                {
                    fileStream.Write(json);
                }

                callback?.Invoke(true);
                Debug.Log($"Game saved successfuly to {path}");
            }
            catch (Exception e)
            {
                LogExseption(e);
                callback?.Invoke(false);
            }
        }


        public void Load<T>(string key, Action<T> callback)
        {
            try
            {
                string path = BuildPath(key);

                if (!File.Exists(path))
                {
                    Debug.LogWarning($"File not found: {path}");
                    callback?.Invoke(default);
                    return;
                }

                JsonSerializerSettings settings = MakeSerializerSettings();

                callback?.Invoke(ReadFile<T>(path, settings));
            }
            catch (Exception e)
            {
                LogExseption(e);
                callback?.Invoke(default);
            }
        }

        public T Load<T>(string key)
        {
            try
            {
                string path = BuildPath(key);

                if (!File.Exists(path))
                {
                    Debug.LogWarning($"File not found: {path}");
                    return default;
                }

                JsonSerializerSettings settings = MakeSerializerSettings();

                return ReadFile<T>(path, settings);
            }
            catch (Exception e)
            {
                LogExseption(e);
                return default;
            }
        }

        private T ReadFile<T>(string path, JsonSerializerSettings settings)
        {
            using (var fileStream = new StreamReader(path))
            {
                var json = fileStream.ReadToEnd();
                var data = JsonConvert.DeserializeObject<T>(json, settings);
                return data;
            }
        }
        private JsonSerializerSettings MakeSerializerSettings()
        {
            return new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented,
                ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver
                {
                    IgnoreSerializableAttribute = true
                }

            };
        }
        private string BuildPath(string key)
        {
            return Path.Combine(Application.persistentDataPath, key);
        }
        private void LogExseption(Exception e)
        {
            #if UNITY_EDITOR
                Debug.LogError($"Save error: {e.Message}");
            #else
                Debug.LogWarning($"Save error: {e.Message}");
            #endif
        }
    }
}
