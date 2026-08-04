using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SaveSystem
{
    public class JsonToFileStorageService : IStorageService
    {
        public void Save(string key, object data, Action<bool> callback = null)
        {
            JsonSerializerSettings settings = MakeSerializerSettings();
            string json = JsonConvert.SerializeObject(data, settings);
            string timestamp = DateTime.UtcNow.ToString("o");

            bool fileSaved = SaveToFile(key, json);
            bool prefsSaved = SaveToPlayerPrefs(key, json, timestamp);

            callback?.Invoke(fileSaved || prefsSaved);
        }

        #region save helpers
        private bool SaveToFile(string key, string json)
        {
            try
            {
                string path = BuildPath(key);
                using (var fileStream = new StreamWriter(path))
                {
                    fileStream.Write(json);
                }

#if UNITY_EDITOR
                Debug.Log($"Game saved successfuly to {path}");
#endif
                return true;
            }
            catch (Exception e)
            {
                LogException(e);
                return false;
            }
        }

        private bool SaveToPlayerPrefs(string key, string json, string timestamp)
        {
            try
            {
                PlayerPrefs.SetString(key, json);
                PlayerPrefs.SetString(BuildTimestampKey(key), timestamp);
                PlayerPrefs.Save();
                return true;
            }
            catch (Exception e)
            {
                LogException(e);
                return false;
            }
        }
        #endregion


        public void Load<T>(string key, Action<T> callback)
        {
            try
            {
                callback?.Invoke(Load<T>(key));
            }
            catch (Exception e)
            {
                LogException(e);
                callback?.Invoke(default);
            }
        }

        public T Load<T>(string key)
        {
            JsonSerializerSettings settings = MakeSerializerSettings();

            DateTime? fileTimestamp = TryGetFileTimestamp(key);
            DateTime? prefsTimestamp = TryGetPlayerPrefsTimestamp(key);

            if (!fileTimestamp.HasValue && !prefsTimestamp.HasValue)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"No save data found for key: {key}");
#endif
                return default;
            }

            bool usePrefs = prefsTimestamp.HasValue && (!fileTimestamp.HasValue || prefsTimestamp.Value > fileTimestamp.Value);

            if (usePrefs)
            {
                T prefsResult = TryLoadFromPlayerPrefs<T>(key, settings);
                if (!EqualityComparer<T>.Default.Equals(prefsResult, default))
                    return prefsResult;

                // Fall back to file IO saving if PlayerPrefs deserialization failed.
                return fileTimestamp.HasValue ? TryLoadFromFile<T>(key, settings) : default;
            }

            T fileResult = TryLoadFromFile<T>(key, settings);
            if (!EqualityComparer<T>.Default.Equals(fileResult, default))
                return fileResult;

            // Fall back to PlayerPrefs if file IO deserialization failed.
            return prefsTimestamp.HasValue ? TryLoadFromPlayerPrefs<T>(key, settings) : default;
        }

        #region helpers
        private DateTime? TryGetFileTimestamp(string key)
        {
            try
            {
                string path = BuildPath(key);
                return File.Exists(path) ? File.GetLastWriteTimeUtc(path) : (DateTime?)null;
            }
            catch (Exception e)
            {
                LogException(e);
                return null;
            }
        }

        private DateTime? TryGetPlayerPrefsTimestamp(string key)
        {
            try
            {
                string timestampKey = BuildTimestampKey(key);
                if (!PlayerPrefs.HasKey(key) || !PlayerPrefs.HasKey(timestampKey))
                    return null;

                return ParseTimestamp(PlayerPrefs.GetString(timestampKey));
            }
            catch (Exception e)
            {
                LogException(e);
                return null;
            }
        }

        private T TryLoadFromFile<T>(string key, JsonSerializerSettings settings)
        {
            try
            {
                string path = BuildPath(key);
                return File.Exists(path) ? ReadFile<T>(path, settings) : default;
            }
            catch (Exception e)
            {
                LogException(e);
                return default;
            }
        }

        private T TryLoadFromPlayerPrefs<T>(string key, JsonSerializerSettings settings)
        {
            try
            {
                string json = PlayerPrefs.GetString(key);
                return JsonConvert.DeserializeObject<T>(json, settings);
            }
            catch (Exception e)
            {
                LogException(e);
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
        private string BuildTimestampKey(string key)
        {
            return key + "_timestamp";
        }
        private DateTime? ParseTimestamp(string timestamp)
        {
            if (DateTime.TryParse(timestamp, null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime result))
            {
                return result;
            }
            return null;
        }
        private void LogException(Exception e)
        {
#if UNITY_EDITOR
            Debug.LogError($"Save error: {e.Message}");
#else
                Debug.LogWarning($"Save error: {e.Message}");
#endif
        }
        #endregion
    }
}