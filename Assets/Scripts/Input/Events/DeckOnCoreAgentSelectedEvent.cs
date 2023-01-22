using Deck.EventManager;
using Deck.Player;

namespace Deck.InputHandling.Events
{
    public class DeckOnCoreAgentSelectedEvent : DeckEvent
    {
        public DeckCoreAgent agent { get; private set; }

        public static DeckOnCoreAgentSelectedEvent Create(DeckCoreAgent agent)
        {
            return new DeckOnCoreAgentSelectedEvent() {agent = agent};
        }
    }
}