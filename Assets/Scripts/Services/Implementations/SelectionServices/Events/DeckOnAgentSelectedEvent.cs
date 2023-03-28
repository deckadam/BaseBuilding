using Deck;
using Deck.EventManager;

namespace Deck.Events
{
    public class DeckOnAgentSelectedEvent : DeckEvent
    {
        public DeckAgent agent { get; private set; }

        public static DeckOnAgentSelectedEvent Create(DeckAgent agent)
        {
            return new()
            {
                agent = agent
            };
        }
    }
}