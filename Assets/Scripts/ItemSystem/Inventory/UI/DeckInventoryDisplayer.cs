using System.Collections.Generic;
using System.Linq;
using Deck.Generalnterfaces;
using Deck.Inventory.Inventory;
using Deck.Inventory.Item;
using Deck.UI;
using Deck.Utility.Logger;
using UnityEngine;

namespace Deck.Inventory.UI
{
    public class DeckInventoryDisplayer : DeckUIBase
    {
        private List<DeckInventoryDisplayerCell> _cells = new();
        private DeckInventory _currentInventory;

        public void SetInventory(IDeckInventoryHolder InventoryHolder)
        {
            Debug.LogError("Setting inventory");
            if (_currentInventory != null)
            {
                _currentInventory.RemoveListener(CreateNewCells);
            }

            _currentInventory = InventoryHolder.GetInventory();
            if (_currentInventory == null)
            {
                DeckLogger.UI("Inventory holder is null");
                return;
            }

            _currentInventory.AddListener(CreateNewCells);
        }

        private void OnDestroy()
        {
            if (_currentInventory == null)
            {
                _currentInventory.RemoveListener(CreateNewCells);
            }
        }

        public override void OnPreAppear()
        {
            if (_currentInventory == null)
            {
                return;
            }

            Debug.LogError(_isAppeared + "   " + isAppearing);
            CreateNewCells(_currentInventory.GetItems());
        }

        private void CreateNewCells(IEnumerable<DeckItem> items)
        {
            if (!_isAppeared && !isAppearing)
            {
                Debug.LogError("Returning");
                return;
            }

            ClearCurrentCells();
            foreach (var deckItem in items)
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