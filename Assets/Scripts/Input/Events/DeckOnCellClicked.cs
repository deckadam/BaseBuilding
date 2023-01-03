using Deck.EventManager;
using Deck.Map;

namespace Deck.InputHandling.Events
{
    public class DeckOnCellClicked : DeckEvent
    {
        public DeckCell deckCell { get; private set; }

        public static DeckOnCellClicked Create(DeckCell cell)
        {
            return new DeckOnCellClicked() {deckCell = cell};
        }
    }
}