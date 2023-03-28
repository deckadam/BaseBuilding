using System.Threading;
using Cysharp.Threading.Tasks;
using Deck.Item;
using Utility;

namespace Deck.Components
{
    public class DeckCommandPickUpItem : DeckCommand
    {
        private DeckComponentInventory _inventory;
        private DeckComponentMovement _movement;
        private DeckItemVisual _itemVisual;

        public DeckCommandPickUpItem(DeckComponentInventory inventory, DeckComponentMovement movement, DeckItemVisual itemVisual)
        {
            _inventory = inventory;
            _movement = movement;
            _itemVisual = itemVisual;
        }

        public override async UniTask<bool> ProcessCommand(CancellationToken token)
        {
            await DeckCommandUtility.AwaitTillDestinationIsReached(_movement, _itemVisual.transform, 2f, token);
            _itemVisual.OnPickUp(_movement.GetComponentHolder().GetCenter());
            _inventory.AddItem(_itemVisual.GetItem());
            return default;
        }
    }
}