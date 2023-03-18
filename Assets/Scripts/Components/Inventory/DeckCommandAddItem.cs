using Cysharp.Threading.Tasks;
using Deck.Components;
using Deck.Data.Item;

namespace Deck.Components
{
    public class DeckCommandAddItem : DeckCommand
    {
        private DeckDataItem _itemToAdd;
        private DeckComponentInventory _target;

        public DeckCommandAddItem(DeckDataItem itemToAdd, DeckComponentInventory target)
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