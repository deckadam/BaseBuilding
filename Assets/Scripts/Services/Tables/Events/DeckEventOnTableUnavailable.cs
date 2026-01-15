using EventManager;

namespace Deck.Services.Tables.Events
{
    public struct DeckEventOnTableUnavailable : IDeckEvent
    {
        public DeckTableWithChairs Table { get; private set; }

        public static DeckEventOnTableUnavailable Create(DeckTableWithChairs table)
        {
            return new DeckEventOnTableUnavailable()
            {
                Table = table
            };
        }
    }
}