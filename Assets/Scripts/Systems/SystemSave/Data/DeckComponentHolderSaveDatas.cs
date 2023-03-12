using System;
using System.Collections.Generic;
using Deck.Components;

namespace Deck.Save.Data
{
    [Serializable]
    public class DeckComponentHolderSaveDatas
    {
        public List<DeckComponentHolderSaveData> datas = new();
    }

    public class DeckComponentHolderSaveData
    {
        public string id;
        public List<DeckComponentSaveData> componentDatas;
    }
}