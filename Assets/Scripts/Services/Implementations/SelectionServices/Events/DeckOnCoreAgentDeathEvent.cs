using Deck.Agent;
using Deck.EventManager;

namespace Deck.Services
{
    public class DeckOnCoreAgentDeathEvent : DeckEvent
    {
        public DeckAgentCore agent { get; private set; }

        public static DeckOnCoreAgentDeathEvent Create(DeckAgentCore agent)
        {
            return new DeckOnCoreAgentDeathEvent
            {
                agent = agent
            };
        }
    }
}