using System.Collections.Generic;
using Deck.Data.UI;
using Deck.Generalnterfaces;
using Deck.Inventory.Item;
using UnityEngine;
using Zenject;

namespace Deck.Inventory.UI
{
    public class DeckInventoryDisplayer : MonoBehaviour
    {
        [Inject] private DeckUIData uiData;
        
        [SerializeField] private CanvasGroup canvasGroup;
        private List<DeckInventoryDisplayerCell> _cells = new();

        public void SetItems(IDeckInventoryHolder InventoryHolder)
        {
            ClearCurrentCells();
            CreateNewCells(InventoryHolder.GetInventory().GetItems());
        }

        private void CreateNewCells(IEnumerable<DeckItem> items)
        {
            foreach (var deckItem in items)
            {
                var newCell = Instantiate(uiData.cellPrefab, transform);
                newCell.Initialize(deckItem);
            }
        }

        private void ClearCurrentCells()
        {
            foreach (var cell in _cells)
            {
                Destroy(cell.gameObject);
            }
        }
    }
}