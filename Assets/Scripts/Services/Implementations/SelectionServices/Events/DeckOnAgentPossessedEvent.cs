using Deck;
using Deck.EventManager;

namespace Deck.Events
{
    public class DeckOnAgentPossessedEvent : DeckEvent
    {
        public DeckAgentCore agent { get; private set; }

        public static DeckOnAgentPossessedEvent Crate(DeckAgentCore agent)
        {
            return new DeckOnAgentPossessedEvent
            {
                agent = agent
            };
        }
    }
}