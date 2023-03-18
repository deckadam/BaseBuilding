using Cysharp.Threading.Tasks;
using Deck.Components;
using Deck.Data.Item;

namespace Deck.Components
{
    public class DeckCommandRemoveItem : DeckCommand
    {
        private DeckDataItem _itemToRemove;
        private DeckComponentInventory _target;

        public DeckCommandRemoveItem(DeckDataItem itemToRemove, DeckComponentInventory target)
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