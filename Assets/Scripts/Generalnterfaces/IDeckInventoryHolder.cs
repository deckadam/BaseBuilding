using Deck.Inventory.Inventory;

namespace Deck.Generalnterfaces
{
    public interface IDeckInventoryHolder
    {
        DeckInventory GetInventory();
    }
}