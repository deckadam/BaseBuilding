using System.Collections.Generic;
using System.Linq;
using Deck.Inventory.Item;
using Deck.MVC.DeckMVCController;
using Deck.Test.MVC.DeckMVCController;
using Deck.UI;
using Deck.Utility.Logger;
using MVC.DeckMVCUIController;
using UnityEngine;

namespace Deck.Inventory.UI
{
    public class DeckInventoryDisplayer : DeckUIBase
    {
        private DeckMVCController<DeckItem> _uiController;
        private List<DeckInventoryDisplayerCell> _cells = new();


        public override void Initialize()
        {
            _uiController = DeckMVC<DeckItem>.GetController();
            _uiController.AddModelListener(CreateNewCells);
            Debug.LogError("INitialized ui");
        }

        public override void DeInitialize()
        {
            _uiController.RemoveModelListener(CreateNewCells);
        }

        public override void OnPreAppear()
        {
            Debug.LogError(_uiController == null);
            var model = _uiController.RequestData();
            if (model == null)
            {
                return;
            }

            CreateNewCells(model);
        }

        private void CreateNewCells(IDeckModel<DeckItem> items)
        {
            if (!_isAppeared && !isAppearing)
            {
                return;
            }

            DeckLogger.UI("Creating item cells");
            var temp = items.Getter();
            Debug.LogError(temp.Count());
            ClearCurrentCells();
            foreach (var deckItem in items.Getter())
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