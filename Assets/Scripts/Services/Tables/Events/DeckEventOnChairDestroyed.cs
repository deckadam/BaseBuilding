using EventManager;
using InGame.Agent.Furniture;

namespace Services.Tables.Events
{
    public struct DeckEventOnChairDestroyed : IDeckEvent
    {
        public DeckAgentChair AgentChair { get; private set; }

        public static DeckEventOnChairDestroyed Create(DeckAgentChair agentChair)
        {
            return new DeckEventOnChairDestroyed()
            {
                AgentChair = agentChair
            };
        }
    }
}