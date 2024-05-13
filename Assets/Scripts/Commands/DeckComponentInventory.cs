using System;
using System.Collections.Generic;
using System.Linq;
using Deck.Data.Item;
using Deck.Item;
using Deck.Save;
using UnityEngine;
using Zenject;

namespace Deck.Commands
{
    public class DeckComponentInventory : DeckComponent
    {
        private Dictionary<DeckDataItem, int> items;
        private Action<Dictionary<DeckDataItem, int>> _listeners;

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
                    if (ownedItem.Key.Name == requiredItem.Item.Name && ownedItem.Value >= requiredItem.RequiredAmount)
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
                    if (ownedItem.Key.Name == requiredItem.Item.Name && ownedItem.Value >= requiredItem.RequiredAmount)
                    {
                        matchCount++;
                    }
                }
            }

            if (matchCount != itemArray.Length)
            {
                return false;
            }

            var matches = new List<(DeckDataItem, int)>();
            foreach (var ownedItem in items)
            {
                foreach (var requiredItem in itemArray)
                {
                    if (ownedItem.Key.Name == requiredItem.Item.Name && ownedItem.Value >= requiredItem.RequiredAmount)
                    {
                        matches.Add((ownedItem.Key, requiredItem.RequiredAmount));
                    }
                }
            }

            foreach (var valueTuple in matches)
            {
                items[valueTuple.Item1] -= valueTuple.Item2;
            }
            
            _listeners?.Invoke(items);
            return true;
        }

        protected override void InternalPreInitialize()
        {
            items = new();
        }

        public void AddItem(DeckDataItem data, int amount = 1)
        {
            if (!items.ContainsKey(data))
            {
                items.Add(data,0);
            }
            
            items[data] += amount;
            _listeners?.Invoke(items);
        }

        public bool RemoveItem(DeckDataItem data)
        {
            var result = items.Remove(data);
            _listeners?.Invoke(items);
            return result;
        }

        public void OnInventoryViewStatusChanged(bool status)
        {
            OnInventoryViewingChanged?.Invoke(status);
        }

        public bool TryGetItemWithTag(List<DeckActionTag> tags, out DeckDataItem result)
        {
            foreach (var item in items)
            {
                if (item.Key.HasTag(tags))
                {
                    result = item.Key;
                    return true;
                }
            }

            result = null;
            return false;
        } 

        public override object GetData()
        {
            var itemData = new ItemData[items.Count];

            var counter = 0;
            foreach (var keyValuePair in items)
            {
                itemData[counter++] = new ItemData(keyValuePair.Key.Name, keyValuePair.Value);
            }

            return new SaveData(itemData);
        }

        public override void LoadData(string value)
        {
            var deserializedData = DeckSaveUtility.GetDeserializedData<SaveData>(value);

            foreach (var itemData in deserializedData.itemData)
            {
                var newItem = _binderItem.GetItemWithName(itemData.name);
                newItem.SetToAmount(itemData.amount);
                items.Add(newItem, itemData.amount);
            }
        }

        public void AddListener(Action<Dictionary<DeckDataItem,int>> listenerToAdd)
        {
            _listeners += listenerToAdd;
        }

        public void RemoveListener(Action<Dictionary<DeckDataItem,int>> listenerToRemove)
        {
            _listeners -= listenerToRemove;
        }

        public Dictionary<DeckDataItem,int> GetItems()
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