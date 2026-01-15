using System;
using System.Collections.Generic;

namespace Systems.SystemSave.Data
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