using System.Collections.Generic;
using Deck.Commands;
using Deck.Data.Item;
using Deck.Services;
using Deck.UI.GamePlay;
using Deck.ItemVisualProviders.Implementations.Inventory;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Deck.UI.Inventory
{
    public class DeckInventoryPopUp : DeckPopUpBase, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private RectTransform cellParent;

        private DeckInventoryDisplayerCell.Factory _inventoryCellFactory;
        private DeckServiceInventory _serviceInventory;
        private List<DeckInventoryDisplayerCell> _cells = new();
        private DeckComponentInventory _componentInventory;

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

            CreateNewCells(componentInventory.GetItems());
        }

        private void CreateNewCells(Dictionary<DeckDataItem, int> items)
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
                newCell.Initialize(deckItem.Key, deckItem.Value, this);
                newCell.gameObject.SetActive(true);
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

        protected override void Despawned()
        {
            ClearCurrentCells();
            _componentInventory.RemoveListener(CreateNewCells);
            _componentInventory.OnInventoryViewStatusChanged(false);
            _componentInventory = null;
        }

        protected override void Spawned()
        {
            _serviceInventory = Deck.GetService<DeckServiceInventory>();
        }

        public DeckComponentInventory GetBindedInventory() => _componentInventory;


        public void OnPointerEnter(PointerEventData eventData)
        {
            _serviceInventory.OnInventoryHoverStart(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _serviceInventory.OnInventoryHoverEnd(this);
        }

        public class Factory : PlaceholderFactory<DeckInventoryPopUp>
        {
        }
    }
}