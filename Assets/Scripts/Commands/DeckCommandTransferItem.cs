using System.Threading;
using Cysharp.Threading.Tasks;
using Deck.Components.Building;
using Deck.Data.Item;
using Deck.Services.CellSelectionService;
using Deck.Utility;

namespace Deck.Components
{
    public class DeckCommandTransferItem : DeckCommand
    {
        private DeckCommandAddItem _addCommand;
        private DeckCommandRemoveItem _removeCommand;
        private DeckComponentInventory _from;
        private DeckComponentInventory _to;
        private DeckDataItem _itemOfInterest;

        public DeckCommandTransferItem(DeckDataItem itemOfInterest,int amount, DeckComponentInventory from, DeckComponentInventory to)
        {
            _addCommand = new DeckCommandAddItem(itemOfInterest,amount, to);
            _removeCommand = new DeckCommandRemoveItem(itemOfInterest, from);
            _from = from;
            _to = to;
        }

        public override async UniTask<bool> ProcessCommand(CancellationToken token)
        {
            DeckServiceSelection.ResetSelectionToPossession();
            var movement = _from.GetAgent().GetDeckComponent<DeckComponentMovement>();

            if (movement == null)
            {
                return default;
            }

            var isCanceled = await DeckCommandUtility.AwaitTillDestinationIsReached(movement, _to.GetAgent().transform, DeckConstantsPrimitive.ITEM_TRANSFER_RANGE, token);
            if (isCanceled)
            {
                return false;
            }
            
            var hasRemoved = await _removeCommand.ProcessCommand(token);
            if (!hasRemoved)
            {
                return false;
            }
            
            await _addCommand.ProcessCommand(token);

            return default;
        }
    }
}