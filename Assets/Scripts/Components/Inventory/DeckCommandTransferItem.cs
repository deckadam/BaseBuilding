using System.Threading;
using Cysharp.Threading.Tasks;
using Deck.Data.Item;
using Deck.Events.CellSelectionService;
using Deck.UI;
using Utility;

namespace Deck.Components
{
    public class DeckCommandTransferItem : DeckCommand
    {
        private DeckCommandAddItem _addCommand;
        private DeckCommandRemoveItem _removeCommand;
        private DeckComponentInventory _from;
        private DeckComponentInventory _to;
        private DeckDataItem _itemOfInterest;

        public DeckCommandTransferItem(DeckDataItem itemOfInterest, DeckComponentInventory from, DeckComponentInventory to)
        {
            _addCommand = new DeckCommandAddItem(itemOfInterest, to);
            _removeCommand = new DeckCommandRemoveItem(itemOfInterest, from);
            _from = from;
            _to = to;
        }

        public override async UniTask<bool> ProcessCommand(CancellationToken token)
        {
            DeckServiceSelection.ResetSelectionToPossession();
            var movement = _from.GetComponentHolder().GetDeckComponent<DeckComponentMovement>();

            if (movement == null)
            {
                return default;
            }

            await DeckCommandUtility.AwaitTillDestinationIsReached(movement, _to.GetComponentHolder().transform, DeckConstantsPrimitive.ITEM_TRANSFER_RANGE, token);

            await _removeCommand.ProcessCommand(token);
            await _addCommand.ProcessCommand(token);

            return default;
        }
    }
}