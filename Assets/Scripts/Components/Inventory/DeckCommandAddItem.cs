using Deck.Components.Operations;
using Deck.Data.Item;

namespace Deck.Inventory
{
    public class DeckCommandAddItem : DeckCommand
    {
        public DeckDataItem itemToAdd { get; }

        public DeckCommandAddItem(DeckDataItem itemToAdd)
        {
            this.itemToAdd = itemToAdd;
            commandType = DeckCommandType.AddItem;
        }
    }
}