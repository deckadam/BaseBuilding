using Deck;
using Deck.EventManager;

namespace Deck.Events
{
    public class DeckOnAgentPossessedEvent : DeckEvent
    {
        public DeckAgent agent { get; private set; }

        public static DeckOnAgentPossessedEvent Crate(DeckAgent agent)
        {
            return new DeckOnAgentPossessedEvent
            {
                agent = agent
            };
        }
    }
}