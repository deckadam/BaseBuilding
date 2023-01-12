using System.Collections.Generic;
using System.Linq;
using Deck.Data.Item;
using Deck.MVC;
using Deck.Utility.Logger;
using UnityEngine;

namespace Deck.UI.Inventory
{
    public class DeckInventoryUI : DeckUIBase
    {
        private DeckMVCController<DeckItem, IEnumerable<DeckItem>> _uiController;
        private List<DeckInventoryDisplayerCell> _cells = new();

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
            Debug.LogError("Create new cells");
            if (!_isAppeared && !isAppearing)
            {
                return;
            }
            Debug.LogError("Passed the checks");
            var temp = items.Getter();
            ClearCurrentCells();
            foreach (var deckItem in temp)
            {
                var newCell = Instantiate(uiData.cellPrefab, transform);
                newCell.Initialize(deckItem);
                _cells.Add(newCell);
            }
        }

        private void ClearCurrentCells()
        {
            foreach (var cell in _cells)
            {
                Destroy(cell.gameObject);
            }

            _cells.Clear();
        }
    }
}