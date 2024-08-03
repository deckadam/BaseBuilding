using System;
using System.Collections.Generic;
using Deck.Commands;
using UnityEngine;
using UnityEngine.Serialization;

namespace Deck.Save.Data
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
        [FormerlySerializedAs("agentGuid")] public int uniqueId;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;
        public List<DeckComponentSaveData> componentDatas;
    }
}