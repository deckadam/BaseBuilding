using System;
using System.Collections.Generic;
using System.Linq;
using Deck.Data.Item;
using UnityEngine;
using Zenject;

namespace Deck.Components
{
    public class DeckComponentInventory : DeckComponent
    {
        private List<DeckDataItem> items;
        private Action<IEnumerable<DeckDataItem>> _listeners;

        private DeckBinderItem _binderItem;

        public Action<bool> OnInventoryViewingChanged;

        [Inject]
        private void Inject(DeckBinderItem binderItem)
        {
            _binderItem = binderItem;
        }

        public bool HasItems(DeckItemRequirement[] itemArray)
        {
            var matchCount = 0;
            foreach (var ownedItem in items)
            {
                foreach (var requiredItem in itemArray)
                {
                    if (ownedItem.Name == requiredItem.Item.Name && ownedItem.Amount >= requiredItem.RequiredAmount)
                    {
                        matchCount++;
                    }
                }
            }

            return matchCount == itemArray.Length;
        }

        public bool ReduceIfPossible(DeckItemRequirement[] itemArray)
        {
            var matchCount = 0;
            foreach (var ownedItem in items)
            {
                foreach (var requiredItem in itemArray)
                {
                    if (ownedItem.Name == requiredItem.Item.Name && ownedItem.Amount >= requiredItem.RequiredAmount)
                    {
                        matchCount++;
                    }
                }
            }

            if (matchCount != itemArray.Length)
            {
                return false;
            }

            foreach (var ownedItem in items)
            {
                foreach (var requiredItem in itemArray)
                {
                    if (ownedItem.Name == requiredItem.Item.Name && ownedItem.Amount >= requiredItem.RequiredAmount)
                    {
                        ownedItem.RemoveFromAmount(requiredItem.RequiredAmount);
                    }
                }
            }

            _listeners?.Invoke(items);
            return true;
        }

        protected override void InternalPreInitialize()
        {
            items = new List<DeckDataItem>();
        }

        public void AddItem(DeckDataItem data)
        {
            var matchingItem = items.FirstOrDefault(item => item.name == data.name);
            if (matchingItem != null)
            {
                matchingItem.AddToAmount(data.Amount);
                _listeners?.Invoke(items);
                return;
            }

            items.Add(data);
            _listeners?.Invoke(items);
        }

        public void RemoveItem(DeckDataItem data)
        {
            items.Remove(data);
            _listeners?.Invoke(items);
        }

        public void OnInventoryViewStatusChanged(bool status)
        {
            OnInventoryViewingChanged?.Invoke(status);
        }

        public override object GetData()
        {
            var itemData = new ItemData[items.Count];
            for (var i = 0; i < items.Count; i++)
            {
                var item = items[i];
                itemData[i] = new ItemData(item.Name, item.Amount);
            }

            return new SaveData(itemData);
        }

        public override void LoadData(string value)
        {
            var deserializedData = JsonUtility.FromJson<SaveData>(value);

            foreach (var itemData in deserializedData.itemData)
            {
                var newItem = _binderItem.GetItemWithName(itemData.name);
                newItem.AddToAmount(itemData.amount);
                items.Add(newItem);
            }
        }

        public void AddListener(Action<IEnumerable<DeckDataItem>> listenerToAdd)
        {
            _listeners += listenerToAdd;
        }

        public void RemoveListener(Action<IEnumerable<DeckDataItem>> listenerToRemove)
        {
            _listeners -= listenerToRemove;
        }

        public IEnumerable<DeckDataItem> GetItems()
        {
            return items;
        }

        [Serializable]
        public class SaveData
        {
            public ItemData[] itemData;

            public SaveData(ItemData[] itemData)
            {
                this.itemData = itemData;
            }
        }

        [Serializable]
        public class ItemData
        {
            public string name;
            public int amount;

            public ItemData(string name, int amount)
            {
                this.name = name;
                this.amount = amount;
            }
        }
    }
}