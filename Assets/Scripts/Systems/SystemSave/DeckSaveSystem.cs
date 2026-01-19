using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;
using Utility;
using SerializationUtility = Sirenix.Serialization.SerializationUtility;

namespace Systems.SystemSave
{
    public static class DeckSaveSystem
    {
        private const string FILE_FORMAT = "MM'.'dd'.'yyyy' 'HH'.'mm'.'ss";
        
        private static SaveData _activeSaveData;

        public static void CreateNewSave()
        {
            _activeSaveData = new SaveData();
        }

        public static void LoadLastSaveData()
        {
            DeckLogger.Save("Trying to load last save file");
            var lastSaveFilePath = Application.persistentDataPath + "/" + GetLastSaveFilePath() + ".dat";
            if (!File.Exists(lastSaveFilePath))
            {
                DeckLogger.Save("Save file does not exist " + lastSaveFilePath);
                return;
            }

            var data = ReadData(lastSaveFilePath);
            _activeSaveData = ConvertToActualData(data);
            DeckLogger.Save("Last save file loaded");
        }

        public static void LoadFromPath(string path)
        {
            if (!File.Exists(path))
            {
                DeckLogger.Save("Save file does not exist " + path);
                return;
            }

            Debug.LogError(path);
            var data = ReadData(path);
            _activeSaveData = ConvertToActualData(data);
            DeckLogger.Save("Last save file loaded");
        }

        public static void DeleteSaveFile(SaveFile saveFile)
        {
            if (!File.Exists(saveFile.path))
            {
                DeckLogger.Save("Save file does not exist");
                return;
            }

            File.Delete(saveFile.path);
            DeckLogger.Save("Save file deleted " + saveFile);
        }

        private static SaveData ConvertToActualData(byte[] data)
        {
            return SerializationUtility.DeserializeValue<SaveData>(data, DataFormat.JSON);
        }

        private static byte[] GetWritableData()
        {
            return SerializationUtility.SerializeValue(_activeSaveData, DataFormat.JSON);
        }

        private static byte[] ReadData(string path)
        {
            return File.ReadAllBytes(path);
        }

        public static bool HasValidSaveFile()
        {
            var lastSaveFilePath = Application.persistentDataPath + "/" + GetLastSaveFilePath() + ".dat";
            var result = File.Exists(lastSaveFilePath);
            DeckLogger.Save("Checking if there is any valid save file result " + result);
            return result;
        }

        private static string GetLastSaveFilePath()
        {
            DeckLogger.Save("Trying to get last save file path");
            var saveFiles = GetAllSaveFiles();
            var splitFileNames = GetSplitFileNames(saveFiles);

            if (splitFileNames == null || !splitFileNames.Any())
            {
                DeckLogger.Save("File path does not exist or does not contain any save file");
                return null;
            }

            var convertedFileNames = splitFileNames.Select(item => DateTime.ParseExact(item, FILE_FORMAT, null).ToString(FILE_FORMAT)).ToList();

            convertedFileNames.Sort();
            convertedFileNames.Reverse();

            DeckLogger.Save("Returning last save file path");
            return convertedFileNames[^1];
        }

        private static FileInfo[] GetAllSaveFiles()
        {
            if (!Directory.Exists(Application.persistentDataPath))
            {
                Directory.CreateDirectory(Application.persistentDataPath);
                DeckLogger.Save("Directory does not exist creating...");

                return null;
            }

            var info = new DirectoryInfo(Application.persistentDataPath);
            var files = info.GetFiles().OrderBy(p => p.CreationTime).Reverse().ToArray();

            DeckLogger.Save("Returning all save file paths");
            return files;
        }

        public static SaveFile[] GetAllSaves()
        {
            var saveFiles = GetAllSaveFiles();
            var splitedFileNames = GetSplitFileNames(saveFiles);
            var result = new SaveFile[saveFiles.Length];

            for (var i = 0; i < saveFiles.Length; i++)
            {
                result[i] = new SaveFile(saveFiles[i].FullName, splitedFileNames[i]);
            }

            return result;
        }

        private static List<string> GetSplitFileNames(IEnumerable<FileInfo> fileNames)
        {
            var result = new List<string>();

            foreach (var fileName in fileNames)
            {
                var splitVersion = fileName.FullName.Split('\\');
                var namePart = splitVersion[^1][..^4];
                result.Add(namePart);
            }

            return result;
        }

        public static void Save()
        {
            var serializedData = GetWritableData();
            File.WriteAllBytes(ConvertToFilePath(GetFileName()), serializedData);
            DeckLogger.Save("Current data saved");
        }

        public static void SetData(string key, object data)
        {
            if (_activeSaveData == null)
            {
                _activeSaveData = new SaveData();
            }

            _activeSaveData[key] = data;
        }

        public static bool HasData(string key)
        {
            return _activeSaveData.ContainsKey(key);
        }

        public static T GetData<T>(string key)
        {
            if (!_activeSaveData.TryGetValue(key, out var value))
            {
                return default;
            }

            return (T)value;
        }

        private static string ConvertToFilePath(string name)
        {
            return Application.persistentDataPath + "/" + name + ".dat";
        }

        private static string GetFileName()
        {
            return DateTime.Now.ToString(FILE_FORMAT);
        }

        #region Test

#if UNITY_EDITOR
        [MenuItem("Deck/Save/Get last save file path")]
        private static void GetLastSaveFilePath_Editor()
        {
            var lastSaveFilePath = GetLastSaveFilePath();
            DeckLogger.Inform(lastSaveFilePath);
        }

        [MenuItem("Deck/Save/Create dummy save file")]
        private static void CreateDummySaveFile_Editor()
        {
            _activeSaveData = new SaveData();
            _activeSaveData["Test"] = "Test";
            var serializedData = GetWritableData();
            File.WriteAllBytes(ConvertToFilePath(GetFileName()), serializedData);
        }

        [MenuItem("Deck/Save/Load last save file")]
        private static void LoadLastSaveData_Editor()
        {
            LoadLastSaveData();
        }
#endif

        #endregion

        [Serializable]
        public class SaveData : Dictionary<string, object>
        {
        }

        public class SaveFile
        {
            public string path { get; }
            public string name { get; }

            public SaveFile(string path, string name)
            {
                this.path = path;
                this.name = name;
            }
        }
    }
}