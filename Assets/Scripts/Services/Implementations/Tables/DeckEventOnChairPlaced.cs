using Deck.Components.Furniture;
using Deck.EventManager;

namespace Services.Implementations.Tables
{
    public struct DeckEventOnChairPlaced : IDeckEvent
    {
        public DeckAgentChair Chair { get; private set; }

        public static DeckEventOnChairPlaced Create(DeckAgentChair chair)
        {
            return new DeckEventOnChairPlaced()
            {
                Chair = chair
            };
        }
    }
}