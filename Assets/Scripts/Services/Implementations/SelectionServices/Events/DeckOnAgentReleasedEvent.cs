using Deck;
using Deck.EventManager;

namespace Deck.Events
{
    public class DeckOnAgentReleasedEvent : DeckEvent
    {
        public DeckAgent agent { get; private set; }

        public static DeckOnAgentReleasedEvent Create(DeckAgent agent)
        {
            return new DeckOnAgentReleasedEvent
            {
                agent = agent
            };
        }
    }
}