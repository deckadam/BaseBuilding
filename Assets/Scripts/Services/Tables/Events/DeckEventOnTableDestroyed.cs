using EventManager;
using InGame.Agent.Furniture;

namespace Services.Tables.Events
{
    public struct DeckEventOnTableDestroyed : IDeckEvent
    {
        public DeckAgentTable AgentTable { get; private set; }

        public static DeckEventOnTableDestroyed Create(DeckAgentTable agentTable)
        {
            return new DeckEventOnTableDestroyed()
            {
                AgentTable = agentTable
            };
        }
    }
}