using Deck.EventManager;
using Deck.InGame.Agent.Furniture;

namespace Deck.Services.Tables.Events
{
    public struct DeckEventOnTableDestroyed : IDeckEvent
    {
        public DeckAgentTable Table { get; private set; }

        public static DeckEventOnTableDestroyed Create(DeckAgentTable table)
        {
            return new DeckEventOnTableDestroyed()
            {
                Table = table
            };
        }
    }
}