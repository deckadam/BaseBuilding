using Deck.EventManager;
using Deck.InGame.Agent.Furniture;

namespace Deck.Services.Tables.Events
{
    public struct DeckEventOnTablePlaced : IDeckEvent
    {
        public DeckAgentAgentTable AgentTable { get; private set; }

        public static DeckEventOnTablePlaced Create(DeckAgentAgentTable agentTable)
        {
            return new DeckEventOnTablePlaced()
            {
                AgentTable = agentTable
            };
        }
    }
}