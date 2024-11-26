using Deck.EventManager;
using Deck.InGame.Agent.Furniture;

namespace Deck.Services.Tables.Events
{
    public struct DeckEventOnTablePlaced : IDeckEvent
    {
        public DeckAgentTable Table { get; private set; }

        public static DeckEventOnTablePlaced Create(DeckAgentTable table)
        {
            return new DeckEventOnTablePlaced()
            {
                Table = table
            };
        }
    }
}