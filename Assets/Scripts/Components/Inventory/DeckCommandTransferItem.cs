using Cysharp.Threading.Tasks;
using Deck.Data.Item;
using Deck.Services.Implementations.CellSelectionService;
using Deck.Utility.Logger;

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
            commandType = DeckCommandType.Movement;
            _addCommand = new DeckCommandAddItem(itemOfInterest, to);
            _removeCommand = new DeckCommandRemoveItem(itemOfInterest, from);
            _from = from;
            _to = to;
        }

        public override async UniTask<bool> ProcessCommand()
        {
            DeckSelectionService.ResetSelectionToPossession();
            var movementComponent = _from.GetComponentHolder().GetDeckComponent<DeckComponentMovement>();
            if (movementComponent == null)
            {
                DeckLogger.Inform("Source of item doesn't contain movement component");
                return await new UniTask<bool>(false);
            }

            await UniTask.WaitWhile(() => movementComponent.SetDestination(_to.GetComponentHolder().transform.position));

            await _removeCommand.ProcessCommand();
            await _addCommand.ProcessCommand();

            return default;
        }
    }
}