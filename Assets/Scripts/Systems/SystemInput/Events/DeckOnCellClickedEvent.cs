using Deck.EventManager;
using Deck.Map;

namespace Deck.InputHandling.Events
{
    public class DeckOnCellClickedEvent : DeckEvent
    {
        public DeckCell deckCell { get; private set; }

        public static DeckOnCellClickedEvent Create(DeckCell cell)
        {
            return new DeckOnCellClickedEvent() {deckCell = cell};
        }
    }
}