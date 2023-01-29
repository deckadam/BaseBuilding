using Deck.Components.Operations;
using Deck.Data.Item;

namespace Deck.Inventory
{
    public class DeckCommandRemoveItem : DeckCommand
    {
        public DeckDataItem itemToRemove { get; }

        public DeckCommandRemoveItem(DeckDataItem itemToRemove)
        {
            commandType = DeckCommandType.RemoveItem;
            this.itemToRemove = itemToRemove;
        }
    }
}