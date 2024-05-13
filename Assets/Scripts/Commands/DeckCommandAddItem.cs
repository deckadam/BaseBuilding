using System.Threading;
using Cysharp.Threading.Tasks;
using Deck.Data.Item;

namespace Deck.Commands
{
    public class DeckCommandAddItem : DeckCommand
    {
        private DeckDataItem _itemToAdd;
        private int _amount;
        private DeckComponentInventory _target;

        public DeckCommandAddItem(DeckDataItem itemToAdd, int amount, DeckComponentInventory target)
        {
            _itemToAdd = itemToAdd;
            _amount = amount;
            _target = target;
        }

        public override UniTask<bool> ProcessCommand(CancellationToken token)
        {
            _target.AddItem(_itemToAdd, _amount);
            return new UniTask<bool>(true);
        }
    }
}