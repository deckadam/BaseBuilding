using Deck.InGame.Agent.Furniture;
using EventManager;

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