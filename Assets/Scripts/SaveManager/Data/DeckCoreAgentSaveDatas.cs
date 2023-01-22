using System;
using System.Collections.Generic;
using Deck.Components;

namespace Deck.SaveService.Data
{
    [Serializable]
    public class DeckCoreAgentSaveDatas
    {
        public List<DeckCoreAgentSaveData> ids;
    }

    public class DeckCoreAgentSaveData
    {
        public string id;
        public List<DeckComponentSaveData> componentDatas;
    }
}