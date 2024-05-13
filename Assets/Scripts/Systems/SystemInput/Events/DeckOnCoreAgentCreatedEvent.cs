using Deck.Agent;
using Deck.EventManager;

namespace Deck.InputHandling.Events
{
    public class DeckOnCoreAgentCreatedEvent : DeckEvent
    {
        public DeckAgentCore agent { get; private set; }

        public static DeckOnCoreAgentCreatedEvent Create(DeckAgentCore agent)
        {
            return new() { agent = agent };
        }
    }
}