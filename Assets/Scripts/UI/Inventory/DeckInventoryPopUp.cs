using System.Collections.Generic;
using Deck.Components;
using Deck.Data.Item;
using Deck.Events;
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
        private DeckServiceInventory _serviceInventory;
        private List<DeckInventoryDisplayerCell> _cells = new();
        private DeckComponentInventory _componentInventory;
        private IMemoryPool _pool;

        [Inject]
        private void Inject(DeckInventoryDisplayerCell.Factory factory)
        {
            _inventoryCellFactory = factory;
        }

        public void SetTarget(DeckComponentInventory componentInventory)
        {
            _componentInventory = componentInventory;
            _componentInventory.AddListener(CreateNewCells);
            _componentInventory.OnInventoryViewStatusChanged(true);

            var gamePlayUIRect = global::Deck.Deck.GetService<DeckServiceUI>().GetUI<DeckGamePlayUI>().GetRectTransform();
            rect.SetParent(gamePlayUIRect, false);
            rect.anchoredPosition = Vector2.zero;

            CreateNewCells(componentInventory.GetItems());
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
            _componentInventory.RemoveListener(CreateNewCells);
            _componentInventory.OnInventoryViewStatusChanged(false);
            _componentInventory = null;
        }

        public void OnSpawned(IMemoryPool p1)
        {
            _pool = p1;
            _serviceInventory = global::Deck.Deck.GetService<DeckServiceInventory>();
        }

        public DeckComponentInventory GetBindedInventory() => _componentInventory;

        public class Factory : PlaceholderFactory<DeckInventoryPopUp>
        {
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _serviceInventory.OnInventoryHoverStart(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _serviceInventory.OnInventoryHoverEnd(this);
        }
    }
}