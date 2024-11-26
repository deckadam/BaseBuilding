using Deck.Components.Furniture;
using Deck.EventManager;

namespace Deck.Services.Tables.Events
{
    public struct DeckEventOnChairDestroyed : IDeckEvent
    {
        public DeckAgentChair Chair { get; private set; }

        public static DeckEventOnChairDestroyed Create(DeckAgentChair chair)
        {
            return new DeckEventOnChairDestroyed()
            {
                Chair = chair
            };
        }
    }
}