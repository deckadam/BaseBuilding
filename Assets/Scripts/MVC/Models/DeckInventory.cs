using System;
using System.Collections.Generic;
using System.Linq;
using Deck.Data.Item;
using Deck.MVC;
using UnityEngine;

namespace Deck.Inventory
{
    public class DeckInventory : MonoBehaviour, IDeckModel<DeckItem, IEnumerable<DeckItem>>
    {
        [SerializeField] private List<DeckItem> items;
        private Action<IDeckModel<DeckItem, IEnumerable<DeckItem>>> _listeners;

        public void Initialize()
        {
            items = new List<DeckItem>();
        }

        public void AddData(DeckItem data)
        {
            items.Add(data);
            _listeners?.Invoke(this);
        }

        public void RemoveData(DeckItem data)
        {
            items.Remove(data);
        }

        public IEnumerable<DeckItem> Getter()
        {
            return items;
        }

        public void Setter(IEnumerable<DeckItem> obj)
        {
            items = obj.ToList();
        }

        public void Register(Action<IDeckModel<DeckItem, IEnumerable<DeckItem>>> listener)
        {
            _listeners += listener;
        }

        public void Unregister(Action<IDeckModel<DeckItem, IEnumerable<DeckItem>>> listener)
        {
            _listeners -= listener;
        }

        public void ClearListeners()
        {
            _listeners = null;
        }
    }
}