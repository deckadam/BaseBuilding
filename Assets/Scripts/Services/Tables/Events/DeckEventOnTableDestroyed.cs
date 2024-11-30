using Deck.EventManager;
using Deck.InGame.Agent.Furniture;

namespace Deck.Services.Tables.Events
{
    public struct DeckEventOnTableDestroyed : IDeckEvent
    {
        public DeckAgentAgentTable AgentTable { get; private set; }

        public static DeckEventOnTableDestroyed Create(DeckAgentAgentTable agentTable)
        {
            return new DeckEventOnTableDestroyed()
            {
                AgentTable = agentTable
            };
        }
    }
}