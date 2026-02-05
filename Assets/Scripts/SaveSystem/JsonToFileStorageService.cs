using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

public class JsonToFileStorageService : IStorageService
{
    public void Save(string key, object data, Action<bool> callback = null)
    {
        try
        {
            string path = BuildPath(key);
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented,
                ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver
                {
                    IgnoreSerializableAttribute = true
                }

            };
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
            Debug.LogError($"Save error: {e.Message}");
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
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver
                {
                    IgnoreSerializableAttribute = true
                }
            };
            using (var fileStream = new StreamReader(path))
            {
                var json = fileStream.ReadToEnd();
                var data = JsonConvert.DeserializeObject<T>(json, settings);
                callback?.Invoke(data);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Load error: {e.Message}");
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
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver
                {
                    IgnoreSerializableAttribute = true
                }
            };
            using (var fileStream = new StreamReader(path))
            {
                var json = fileStream.ReadToEnd();
                var data = JsonConvert.DeserializeObject<T>(json, settings);
                return data;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Load error: {e.Message}");
            return default;
        }
    }
    private string BuildPath(string key)
    {
        return Path.Combine(Application.persistentDataPath, key);
    }
}
