using System;
using System.Collections.Generic;
using Deck.Components;
using UnityEngine;
using UnityEngine.Serialization;

namespace Deck.Save
{
    [Serializable]
    public class DeckComponentHolderSaveDatas
    {
        public List<DeckComponentHolderSaveData> datas = new();
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