using EventManager;

namespace Deck.Services.Tables.Events
{
    public struct DeckEventOnTableAvailable : IDeckEvent
    {
        public DeckTableWithChairs Table { get; set; }

        public static DeckEventOnTableAvailable Create(DeckTableWithChairs table)
        {
            return new DeckEventOnTableAvailable()
            {
                Table = table
            };
        }
    }
}