using System.Collections.Generic;
using Deck.Data.Item;
using Deck.MVC;
using Zenject;

namespace Deck.UI.Inventory
{
    public class DeckInventoryUI : DeckUIBase
    {
        private DeckMVCController<DeckItem, IEnumerable<DeckItem>> _uiController;
        private List<DeckInventoryDisplayerCell> _cells = new();
        private DeckInventoryDisplayerCell.Factory _inventoryCellFactory;


        [Inject]
        private void Inject(DeckInventoryDisplayerCell.Factory factory)
        {
            _inventoryCellFactory = factory;
        }

        public override void Initialize()
        {
            _uiController = DeckMVC<DeckItem, IEnumerable<DeckItem>>.GetController();
            _uiController.AddModelListener(CreateNewCells);
        }

        public override void DeInitialize()
        {
            _uiController.RemoveModelListener(CreateNewCells);
        }

        public override void OnPreAppear()
        {
            var model = _uiController.RequestData();
            if (model == null)
            {
                return;
            }

            CreateNewCells(model);
        }

        private void CreateNewCells(IDeckModel<DeckItem, IEnumerable<DeckItem>> items)
        {
            if (!_isAppeared && !isAppearing)
            {
                return;
            }

            ClearCurrentCells();

            if (items == null)
            {
                return;
            }

            var temp = items.Getter();
            foreach (var deckItem in temp)
            {
                var newCell = _inventoryCellFactory.Create();
                newCell.transform.SetParent(transform);
                newCell.Initialize(deckItem);
                _cells.Add(newCell);
            }
        }

        private void ClearCurrentCells()
        {
            foreach (var cell in _cells)
            {
                cell.Despawn();
            }

            _cells.Clear();
        }
    }
}