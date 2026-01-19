using System;
using System.Collections.Generic;
using Base;
using UnityEngine;
using UnityEngine.Serialization;

namespace Systems.SystemSave.Data
{
    [Serializable]
    public class DeckComponentHolderSaveDatas
    {
        public List<DeckComponentHolderSaveData> data = new();
    }

    [Serializable]
    public class DeckComponentHolderSaveData
    {
        public int prefabId;
        public int uniqueId;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;
        public List<DeckComponentSaveData> componentDatas;
        public string additionalData;
    }
}