using System.IO;
using Core;
using UnityEngine;

namespace Services
{
    public class JsonSaveService
    {
        private readonly string _storagePath;

        public JsonSaveService()
        {
            _storagePath = Path.Combine(Application.persistentDataPath, StorageKeys.StorageFolder);
            Directory.CreateDirectory(_storagePath);
        }

        public void Save<T>(string key, T data)
        {
            string json = JsonUtility.ToJson(data);
            File.WriteAllText(GetPath(key), json);
        }

        public T Load<T>(string key) where T : class
        {
            string path = GetPath(key);
            if (!File.Exists(path)) return null;

            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<T>(json);
        }

        private string GetPath(string key)
        {
            return Path.Combine(_storagePath, key + ".json");
        }
    }
}
