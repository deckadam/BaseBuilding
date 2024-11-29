using Deck.Commands;
using Deck.UI.Inventory;

namespace Deck.ItemVisualProviders.Inventory
{
    public class DeckServiceInventory : DeckServiceBase
    {
        private DeckInventoryDisplayerCell _cell;
        private DeckInventoryPopUp _hovered;

        public void OnDragBegin(DeckInventoryDisplayerCell cell)
        {
            _cell = cell;
        }

        public void OnInventoryHoverStart(DeckInventoryPopUp hoverStarted)
        {
            _hovered = hoverStarted;
        }

        public void OnInventoryHoverEnd(DeckInventoryPopUp hoverEnd)
        {
            if (_hovered == hoverEnd)
            {
                _hovered = null;
            }
        }

        public bool TryToPlace()
        {
            if (_hovered == _cell.GetPopUp())
            {
                return true;
            }

            if (_hovered == null)
            {
                return false;
            }

            var amount = _cell.GetAmount();
            var item = _cell.GetItem();
            var inventory = _cell.GetPopUp().GetBindedInventory();
            var agent = inventory.GetAgent();

            agent.AddCommand(new DeckCommandTransferItem(item, amount, inventory, _hovered.GetBindedInventory()), false);

            return true;
        }
    }
}