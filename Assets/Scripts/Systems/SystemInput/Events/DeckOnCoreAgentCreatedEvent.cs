using Deck.EventManager;
using Deck.Agent;

namespace Deck.InputHandling.Events
{
    public class DeckOnCoreAgentCreatedEvent : DeckEvent
    {
        public DeckCoreAgent agent { get; private set; }

        public static DeckOnCoreAgentCreatedEvent Create(DeckCoreAgent agent)
        {
            return new() {agent = agent};
        }
    }
}