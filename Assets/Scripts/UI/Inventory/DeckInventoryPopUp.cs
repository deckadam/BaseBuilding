using System.Collections.Generic;
using Deck.Data.Item;
using Deck.ItemVisualProviders.Implementations.Inventory;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Deck.Components.Building.Inventory
{
    public class DeckInventoryPopUp : DeckPopUpBase, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private RectTransform cellParent;

        private DeckServiceInventory _serviceInventory;
        private List<DeckInventoryDisplayerCell> _cells = new();
        private DeckComponentInventory _componentInventory;

        public void SetTarget(DeckComponentInventory componentInventory)
        {
            _serviceInventory = Deck.GetService<DeckServiceInventory>();
            
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
                var newCell = InstanceProvider.RentUIElement<DeckInventoryDisplayerCell>();
                newCell.transform.SetParent(cellParent, false);
                newCell.Initialize(deckItem.Key, deckItem.Value, this);
                newCell.gameObject.SetActive(true);
                _cells.Add(newCell);
            }
        }

        private void ClearCurrentCells()
        {
            InstanceProvider.ReturnUIElement(_cells);
            _cells.Clear();
        }

        protected override void InternalOnDespawned()
        {
            ClearCurrentCells();
            _componentInventory.RemoveListener(CreateNewCells);
            _componentInventory.OnInventoryViewStatusChanged(false);
            _componentInventory = null;
        }

        protected override void InternalOnSpawned()
        {
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
    }
}