using Deck.Components;
using Deck.Inventory;

namespace Deck.Test.General
{
    public interface IDeckSelectable
    {
        DeckSelectOperations[] GetAvailableOperations();

        void OnPossesStarted();
        void OnPossesFinished();
        DeckInventoryComponent GetInventory();
        DeckHealthComponent GetHealthComponent();
    }
}