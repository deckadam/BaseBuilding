using Cysharp.Threading.Tasks;
using Deck.Components;
using Deck.Components.Operations;
using Deck.Data.Item;
using Deck.Services.Implementations.CellSelectionService;
using Deck.Utility.Logger;

namespace Deck.Inventory
{
    public class DeckCommandTransferItem : DeckCommand
    {
        private DeckCommandAddItem _addCommand;
        private DeckCommandRemoveItem _removeCommand;
        private DeckInventoryComponent _from;
        private DeckInventoryComponent _to;
        private DeckDataItem _itemOfInterest;

        public DeckCommandTransferItem(DeckDataItem itemOfInterest, DeckInventoryComponent from, DeckInventoryComponent to)
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
            var movementComponent = _from.GetComponentHolder().GetDeckComponent<DeckMovementComponent>();
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