using System;
using System.Collections.Generic;
using System.IO;
using iznakurnoz.Bot.Interfaces;
using System.Text.Json;

namespace iznakurnoz.Bot.Services
{
    /// <summary>
    /// Хранилище произвольных данных бота.
    /// </summary>
    public class DataStorage : IDataStorage
    {        
        /// <summary>
        /// Наименование файла в котором хранится информация хранилища.ё
        /// </summary>
        private const string DataStorageFilename = "datastorage.json";

        private IDictionary<string, object> _storage = null;
        private IDictionary<string, string> _readedStorage = null;

        public T Get<T>(string key)
        {
            InitializeStorage();

            if (_storage.TryGetValue(key, out var storageObject))
            {
                return (T)storageObject;
            }

            if (_readedStorage.TryGetValue(key, out var readedObject))
            {
                var obj = JsonSerializer.Deserialize<T>(readedObject);
                _storage[key] = obj;

                return obj;
            }

            return Activator.CreateInstance<T>();
        }

        public void Set<T>(string key, T obj)
        {
            InitializeStorage();

            _storage[key] = obj;

            var filePath = GetFilePath();
            var options = new JsonSerializerOptions()
            {
                WriteIndented = true                
            };
            var storageContent = JsonSerializer.Serialize(_storage, options);
            File.WriteAllText(filePath, storageContent);
        }

        private void InitializeStorage()
        {
            if (_storage != null)
            {
                return;
            }

            var filePath = GetFilePath();

            if (File.Exists(filePath))
            {
                var storageContent = File.ReadAllText(filePath);
                var readedStorage = JsonSerializer.Deserialize<IDictionary<string, object>>(storageContent);
                _readedStorage = new Dictionary<string, string>();
                
                foreach (var pair in readedStorage)
                {
                    _readedStorage.Add(pair.Key, pair.Value.ToString());
                }
            }
            
            _storage = new Dictionary<string, object>();
        }

        private static string GetFilePath()
        {
            return FilePathProvider.GetFilePath(DataStorageFilename);
        }
    }
}