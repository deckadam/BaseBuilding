using Deck;
using Deck.EventManager;

namespace Deck.Events
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