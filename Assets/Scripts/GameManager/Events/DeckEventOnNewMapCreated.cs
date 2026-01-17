using EventManager;
using InGame.Map.Data;

namespace GameManager.Events
{
    public struct DeckEventOnNewMapCreated : IDeckEvent
    {
        public DeckDataMap mapData { get; private set; }

        public static DeckEventOnNewMapCreated Create(DeckDataMap mapData)
        {
            return new DeckEventOnNewMapCreated()
            {
                mapData = mapData
            };
        }
    }
}