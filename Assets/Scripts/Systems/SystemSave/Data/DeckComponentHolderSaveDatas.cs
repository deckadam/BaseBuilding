using System;
using System.Collections.Generic;
using Deck.Commands;

namespace Deck.Save.Data
{
    [Serializable]
    public class DeckComponentHolderSaveDatas
    {
        public List<DeckComponentHolderSaveData> datas = new();
    }

    public class DeckComponentHolderSaveData
    {
        public string prefabId;
        public string agentGuid;
        public List<DeckComponentSaveData> componentDatas;
    }
}