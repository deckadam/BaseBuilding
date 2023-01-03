using Deck.EventManager;
using Deck.Player;

namespace Deck.InputHandling.Events
{
    public class DeckOnCoreAgentSelected : DeckEvent
    {
        public DeckCoreAgent agent { get; private set; }

        public static DeckOnCoreAgentSelected Create(DeckCoreAgent agent)
        {
            return new DeckOnCoreAgentSelected() {agent = agent};
        }
    }
}