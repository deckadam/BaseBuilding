using System.Threading;
using Cysharp.Threading.Tasks;
using Deck.Components;
using Deck.Data.Item;

namespace Deck.Commands
{
    public class DeckCommandRemoveItem : DeckCommand
    {
        private DeckDataItem _itemToRemove;
        private DeckComponentInventory _target;

        public DeckCommandRemoveItem(DeckDataItem itemToRemove, DeckComponentInventory target)
        {
            _itemToRemove = itemToRemove;
            _target = target;
        }

        public override UniTask<bool> ProcessCommand(CancellationToken token)
        {
            var result = _target.RemoveItem(_itemToRemove);
            return result ? UniTask.FromResult(true) : UniTask.FromResult(false);
        }
    }
}