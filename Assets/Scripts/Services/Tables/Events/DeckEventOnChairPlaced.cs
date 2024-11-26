using Deck.Components.Furniture;
using Deck.EventManager;

namespace Deck.Services.Tables.Events
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