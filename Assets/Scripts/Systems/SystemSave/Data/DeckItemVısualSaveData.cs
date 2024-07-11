using System;
using System.Collections.Generic;
using UnityEngine;

namespace Deck.Save.Data
{
    [Serializable]
    public struct DeckItemVisualSaveDatas
    {
        public List<DeckItemVisualSaveDataPair> saveDatas;

        public DeckItemVisualSaveDatas(List<DeckItemVisualSaveDataPair> saveDatas)
        {
            this.saveDatas = saveDatas;
        }
    }

    [Serializable]
    public struct DeckItemVisualSaveDataPair
    {
        public string typeName;
        public string saveData;
    }
}