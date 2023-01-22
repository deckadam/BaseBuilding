using System;
using Deck.Component;

namespace Deck.Components
{
    public interface IDeckComponent
    {
        void Initialize(DeckComponentHolder holder);
        void DeInitialize();
        void Tick();
        DeckComponentHolder GetComponentOwner();
        object GetData();
        void LoadData(string value);
    }

    [Serializable]
    public class DeckComponentSaveData
    {
        public string id;
        public string data;
    }
}