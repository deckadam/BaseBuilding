using Deck.InGame.Agent.Furniture;
using EventManager;

namespace Deck.Services.Tables.Events
{
    public struct DeckEventOnChairDestroyed : IDeckEvent
    {
        public DeckAgentAgentChair AgentChair { get; private set; }

        public static DeckEventOnChairDestroyed Create(DeckAgentAgentChair agentChair)
        {
            return new DeckEventOnChairDestroyed()
            {
                AgentChair = agentChair
            };
        }
    }
}