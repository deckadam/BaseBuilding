using System.Threading;
using Cysharp.Threading.Tasks;
using Deck.Data.Item;
using Deck.Components;

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
        }

        public override UniTask<bool> ProcessCommand(CancellationToken token)
        {
            _target.AddItem(_itemToAdd);
            return default;
        }
    }
}