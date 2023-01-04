using System;
using System.Collections.Generic;
using Deck.Generalnterfaces;
using Deck.Inventory.Item;
using UnityEngine;

namespace Deck.Inventory.Inventory
{
    public class DeckInventory : MonoBehaviour, DeckObservable<IEnumerable<DeckItem>>
    {
        [SerializeField] private List<DeckItem> items = new();
        private Action<IEnumerable<DeckItem>> _listeners;

        public void AddItem(DeckItem deckItemToAdd)
        {
            items.Add(deckItemToAdd);
            _listeners?.Invoke(items);
        }

        public void RemoveItem(DeckItem deckItemToRemove)
        {
            items.Remove(deckItemToRemove);
            _listeners?.Invoke(items);
        }

        public IEnumerable<DeckItem> GetItems()
        {
            return items;
        }

        public void AddListener(Action<IEnumerable<DeckItem>> listener)
        {
            _listeners += listener;
        }

        public void RemoveListener(Action<IEnumerable<DeckItem>> listener)
        {
            _listeners -= listener;
        }

        public void ClearListeners()
        {
            _listeners = null;
        }
    }
}