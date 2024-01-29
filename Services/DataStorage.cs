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
    internal class DataStorage : IDataStorage
    {
        /// <summary>
        /// Наименование файла в котором хранится информация хранилища.
        /// </summary>
        private const string DataStorageFilename = "datastorage.json";

        private Dictionary<string, object> _storage;
        private Dictionary<string, string> _readedStorage;
        private string _dataFilePath;
        private readonly FilePathProvider _filePathProvider;
        private static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions()
        {
            WriteIndented = true
        };

        public DataStorage(FilePathProvider filePathProvider)
        {
            _filePathProvider = filePathProvider;
        }

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
            var storageContent = JsonSerializer.Serialize(_storage, SerializerOptions);
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

        private string GetFilePath()
        {
            if (string.IsNullOrWhiteSpace(_dataFilePath))
            {
                _dataFilePath = Path.Combine(_filePathProvider.GetDataDirectoryPath(), DataStorageFilename);
            }

            return _dataFilePath;
        }
    }
}