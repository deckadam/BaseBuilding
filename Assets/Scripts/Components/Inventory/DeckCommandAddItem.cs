using Cysharp.Threading.Tasks;
using Deck.Components.Operations;
using Deck.Data.Item;

namespace Deck.Inventory
{
    public class DeckCommandAddItem : DeckCommand
    {
        private DeckDataItem _itemToAdd;
        private DeckInventoryComponent _target;

        public DeckCommandAddItem(DeckDataItem itemToAdd, DeckInventoryComponent target)
        {
            _itemToAdd = itemToAdd;
            _target = target;
            commandType = DeckCommandType.AddItem;
        }

        public override UniTask<bool> ProcessCommand()
        {
            _target.AddItem(_itemToAdd);
            return default;
        }
    }
}