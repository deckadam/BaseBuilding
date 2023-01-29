using Deck.Component;
using Deck.EventManager;

namespace Deck.Services.Implementations.CellSelectionService.Events
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