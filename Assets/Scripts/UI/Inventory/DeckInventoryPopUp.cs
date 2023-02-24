using System.Collections.Generic;
using Deck.Data.Item;
using Deck.Inventory;
using Deck.Services.Implementations;
using Deck.UI.GamePlay;
using Services.Implementations.Inventory;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Deck.UI.Inventory
{
    public class DeckInventoryPopUp : DeckPopUpBase, IPoolable<IMemoryPool>, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private RectTransform cellParent;

        private DeckInventoryDisplayerCell.Factory _inventoryCellFactory;
        private DeckInventoryService _inventoryService;
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
            rect.SetParent(gamePlayUIRect, false);
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
                newCell.transform.SetParent(cellParent, false);
                newCell.Initialize(deckItem, this);
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
            _pool?.Despawn(this);
        }

        public void OnDespawned()
        {
            _pool = null;
            ClearCurrentCells();
            _inventoryComponent.RemoveListener(CreateNewCells);
            _inventoryComponent = null;
        }

        public void OnSpawned(IMemoryPool p1)
        {
            _pool = p1;
            _inventoryService = Deck.GetService<DeckInventoryService>();
        }

        public DeckInventoryComponent GetBindedInventory() => _inventoryComponent;

        public class Factory : PlaceholderFactory<DeckInventoryPopUp>
        {
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _inventoryService.OnInventoryHoverStart(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _inventoryService.OnInventoryHoverEnd(this);
        }
    }
}