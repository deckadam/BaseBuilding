using Deck.Agent;
using Deck.EventManager;

namespace Deck.Services
{
    public class DeckOnSelectionReleasedEvent : DeckEvent
    {
        public DeckAgent agent { get; private set; }

        public static DeckOnSelectionReleasedEvent Create(DeckAgent agent)
        {
            return new()
            {
                agent = agent
            };
        }
    }
}