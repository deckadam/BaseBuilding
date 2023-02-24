using System;
using System.Collections.Generic;
using Deck.Components;
using Deck.Data.Item;
using UnityEngine;
using Zenject;

namespace Deck.Inventory
{
    public class DeckInventoryComponent : DeckComponent
    {
        private List<DeckDataItem> items;
        private Action<IEnumerable<DeckDataItem>> _listeners;

        private DeckBinderItem _binderItem;

        [Inject]
        private void Inject(DeckBinderItem binderItem)
        {
            _binderItem = binderItem;
        }

        protected override void Initialize()
        {
            items = new List<DeckDataItem>();
        }

        public void AddItem(DeckDataItem data)
        {
            items.Add(data);
            _listeners?.Invoke(items);
        }

        public void RemoveItem(DeckDataItem data)
        {
            Debug.LogError("REMOVE");
            items.Remove(data);
            _listeners?.Invoke(items);
        }

        public override object GetData()
        {
            var itemData = new ItemData[items.Count];
            for (var i = 0; i < items.Count; i++)
            {
                var item = items[i];
                itemData[i] = new ItemData(item.name, item.amount);
            }

            return new SaveData(itemData);
        }

        public override void LoadData(string value)
        {
            var deserializedData = JsonUtility.FromJson<SaveData>(value);

            foreach (var itemData in deserializedData.itemData)
            {
                var newItem = _binderItem.GetItemWithName(itemData.name);
                newItem.amount = itemData.amount;
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