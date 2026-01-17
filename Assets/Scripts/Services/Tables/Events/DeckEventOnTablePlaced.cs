using EventManager;
using InGame.Agent.Furniture;

namespace Services.Tables.Events
{
    public struct DeckEventOnTablePlaced : IDeckEvent
    {
        public DeckAgentTable AgentTable { get; private set; }

        public static DeckEventOnTablePlaced Create(DeckAgentTable agentTable)
        {
            return new DeckEventOnTablePlaced()
            {
                AgentTable = agentTable
            };
        }
    }
}