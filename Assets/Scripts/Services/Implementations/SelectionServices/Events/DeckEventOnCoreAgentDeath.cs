using Deck.Agent;
using Deck.EventManager;

namespace Deck.Services
{
    public class DeckEventOnCoreAgentDeath : DeckEvent
    {
        public DeckAgentCore agent { get; private set; }

        public static DeckEventOnCoreAgentDeath Create(DeckAgentCore agent)
        {
            return new DeckEventOnCoreAgentDeath
            {
                agent = agent
            };
        }
    }
}