using System.Collections.Generic;
using Deck.Data.Item;
using Deck.Inventory;
using Deck.Services;
using Deck.Services.Implementations;
using Deck.UI.GamePlay;
using UnityEngine;
using Zenject;

namespace Deck.UI.Inventory
{
    public class DeckInventoryPopUp : DeckPopUpBase, IPoolable<IMemoryPool>
    {
        [SerializeField] private RectTransform cellParent;
        private DeckInventoryDisplayerCell.Factory _inventoryCellFactory;
        private List<DeckInventoryDisplayerCell> _cells = new();
        private DeckInventoryComponent _inventoryComponent;
        private IMemoryPool _pool;

        [Inject]
        private void Inject(DeckInventoryDisplayerCell.Factory factory)
        {
            _inventoryCellFactory = factory;
        }

        public void SetTarget(DeckInventoryComponent inventoryComponent)
        {
            _inventoryComponent = inventoryComponent;
            _inventoryComponent.AddListener(CreateNewCells);

            var gamePlayUIRect = Deck.GetService<DeckUIService>().GetUI<DeckGamePlayUI>().GetRectTransform();
            rect.SetParent(gamePlayUIRect);
            rect.anchoredPosition = Vector2.zero;

            CreateNewCells(inventoryComponent.GetItems());
        }

        private void CreateNewCells(IEnumerable<DeckDataItem> items)
        {
            ClearCurrentCells();

            if (items == null)
            {
                return;
            }

            foreach (var deckItem in items)
            {
                var newCell = _inventoryCellFactory.Create();
                newCell.transform.SetParent(cellParent);
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

        public void OnCloseRequested()
        {
            _pool.Despawn(this);
        }

        public void OnDespawned()
        {
            ClearCurrentCells();
            _inventoryComponent.RemoveListener(CreateNewCells);
            _inventoryComponent = null;
        }

        public void OnSpawned(IMemoryPool p1)
        {
            _pool = p1;
        }

        public class Factory : PlaceholderFactory<DeckInventoryPopUp>
        {
        }
    }
}