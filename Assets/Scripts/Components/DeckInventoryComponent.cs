using System;
using System.Collections.Generic;
using Deck.Component;
using Deck.Components;
using Deck.Data.Item;
using UnityEngine;
using Zenject;

namespace Deck.Inventory
{
    public class DeckInventoryComponent : IDeckComponent
    {
        private List<DeckDataItem> items;
        private Action<IEnumerable<DeckDataItem>> _listeners;
        private DeckComponentHolder _holder;

        private DeckBinderItem _binderItem;

        [Inject]
        private void Inject(DeckBinderItem binderItem)
        {
            _binderItem = binderItem;
        }

        public void Initialize(DeckComponentHolder holder)
        {
            items = new List<DeckDataItem>();
            _holder = holder;
        }

        public void AddItem(DeckDataItem data)
        {
            items.Add(data);
            _listeners?.Invoke(items);
        }

        public void RemoveItem(DeckDataItem data)
        {
            items.Remove(data);
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

        public void DeInitialize()
        {
        }

        public void Tick()
        {
        }

        public DeckComponentHolder GetComponentOwner()
        {
            return _holder;
        }

        public object GetData()
        {
            var itemData = new ItemData[items.Count];
            for (var i = 0; i < items.Count; i++)
            {
                var item = items[i];
                itemData[i] = new ItemData(item.name, item.amount);
            }

            return new SaveData(itemData);
        }

        public void LoadData(string value)
        {
            var deserializedData = JsonUtility.FromJson<SaveData>(value);

            foreach (var itemData in deserializedData.itemData)
            {
                var newItem = _binderItem.GetItemWithName(itemData.name);
                newItem.amount = itemData.amount;
                items.Add(newItem);
            }
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