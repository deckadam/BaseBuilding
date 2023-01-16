using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Deck.Utility.Logger;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;
using SerializationUtility = Sirenix.Serialization.SerializationUtility;

namespace Deck.Services.Implementations.SaveService
{
    public class DeckSaveManager
    {
        private static readonly string FILE_PATH = Application.persistentDataPath + "/" + "SaveData.dat";
        private static readonly string FILE_FORMAT = "MM'.'dd'.'yyyy' 'HH'.'mm'.'ss";
        private static SaveData _activeSaveData;

        public static void LoadLastSaveData()
        {
            DeckLogger.Save("Trying to load last save file");
            var lastSaveFilePath = Application.persistentDataPath + "/" + GetLastSaveFilePath() + ".dat";
            if (!File.Exists(lastSaveFilePath))
            {
                DeckLogger.Save("Save file does not exist " + lastSaveFilePath);
                return;
            }

            var data = File.ReadAllBytes(lastSaveFilePath);
            _activeSaveData = SerializationUtility.DeserializeValue<SaveData>(data, DataFormat.JSON);
            DeckLogger.Save("Last save file loaded");
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
            var saveFiles = GetAllSaveFiles();
            var splitedFileNames = GetSplitedFileNames(saveFiles);

            if (splitedFileNames == null || !splitedFileNames.Any())
            {
                return null;
            }

            var convertedFileNames = splitedFileNames.Select(item => DateTime.ParseExact(item, FILE_FORMAT, null).ToString(FILE_FORMAT)).ToList();

            convertedFileNames.Sort();

            return convertedFileNames[^1];
        }

        private static IEnumerable<string> GetAllSaveFiles()
        {
            if (!Directory.Exists(Application.persistentDataPath))
            {
                Directory.CreateDirectory(Application.persistentDataPath);
                return null;
            }

            return Directory.GetFiles(Application.persistentDataPath);
        }

        private static List<string> GetSplitedFileNames(IEnumerable<string> fileNames)
        {
            var result = new List<string>();

            foreach (var fileName in fileNames)
            {
                var splitVersion = fileName.Split('\\');
                var namePart = splitVersion[^1].Substring(0, splitVersion[^1].Length - 4);
                result.Add(namePart);
            }

            return result;
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
            GetLastSaveFilePath();
        }

        [MenuItem("Deck/Save/Create dummy save file")]
        private static void CreateDummySaveFile_Editor()
        {
            _activeSaveData = new SaveData();
            _activeSaveData["Test"] = "lorem ipsum";
            var serializedData = SerializationUtility.SerializeValue(_activeSaveData, DataFormat.JSON);
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
    }
}