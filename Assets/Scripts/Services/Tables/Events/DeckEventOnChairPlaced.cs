using Deck.InGame.Agent.Furniture;
using EventManager;

namespace Deck.Services.Tables.Events
{
    public struct DeckEventOnChairPlaced : IDeckEvent
    {
        public DeckAgentAgentChair AgentChair { get; private set; }

        public static DeckEventOnChairPlaced Create(DeckAgentAgentChair agentChair)
        {
            return new DeckEventOnChairPlaced()
            {
                AgentChair = agentChair
            };
        }
    }
}