using Deck.Components;
using Deck.Services;
using Deck.Utility.Constants.Inventory;

namespace Services.Implementations.Inventory
{
    public class DeckInventoryService : DeckServiceBase
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

            var item = _cell.GetItem();
            var inventory = _cell.GetPopUp().GetBindedInventory();
            var agent = inventory.GetComponentHolder();
            
            agent.AddCommand(new DeckCommandTransferItem(item, inventory, _hovered.GetBindedInventory()));
            
            return true;
        }
    }
}