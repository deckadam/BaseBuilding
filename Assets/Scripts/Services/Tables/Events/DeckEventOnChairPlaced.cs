using EventManager;
using InGame.Agent.Furniture;

namespace Services.Tables.Events
{
    public struct DeckEventOnChairPlaced : IDeckEvent
    {
        public DeckAgentChair AgentChair { get; private set; }

        public static DeckEventOnChairPlaced Create(DeckAgentChair agentChair)
        {
            return new DeckEventOnChairPlaced()
            {
                AgentChair = agentChair
            };
        }
    }
}