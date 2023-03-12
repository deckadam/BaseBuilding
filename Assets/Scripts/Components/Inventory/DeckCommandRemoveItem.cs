using Cysharp.Threading.Tasks;
using Deck.Components.Operations;
using Deck.Data.Item;

namespace Deck.Inventory
{
    public class DeckCommandRemoveItem : DeckCommand
    {
        private DeckDataItem _itemToRemove;
        private DeckInventoryComponent _target;

        public DeckCommandRemoveItem(DeckDataItem itemToRemove, DeckInventoryComponent target)
        {
            commandType = DeckCommandType.RemoveItem;
            _itemToRemove = itemToRemove;
            _target = target;
        }

        public override UniTask<bool> ProcessCommand()
        {
            _target.RemoveItem(_itemToRemove);
            return default;
        }
    }
}