using System;
using System.Collections.Generic;
using UnityEngine;

namespace Deck.Save.Data
{
    [Serializable]
    public class DeckItemVisualSaveDatas
    {
        public List<DeckItemVisualSaveData> datas = new();
    }

    [Serializable]
    public class DeckItemVisualSaveData
    {
        public int prefabId;
        public int uniqueId;
        public Vector3 position;
        public Vector3 rotation;
        public bool isOnTheGround;
        public object additionalData;
    }
}