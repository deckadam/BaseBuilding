using System;
using System.Collections.Generic;
using System.Linq;
using Deck.Inventory.Item;
using Deck.MVC.DeckMVCController;
using UnityEngine;

namespace Deck.Inventory.Inventory
{
    public class DeckInventory : MonoBehaviour, IDeckModel<DeckItem>
    {
        [SerializeField] private List<DeckItem> items = new();
        private Action<IDeckModel<DeckItem>> _listeners;

        public void AddItem(DeckItem deckItemToAdd)
        {
            items.Add(deckItemToAdd);
            _listeners?.Invoke(this);
        }

        public void RemoveItem(DeckItem deckItemToRemove)
        {
            items.Remove(deckItemToRemove);
            _listeners?.Invoke(this);
        }


        public IEnumerable<DeckItem> Getter()
        {
            return items;
        }

        public void Setter(IEnumerable<DeckItem> obj)
        {
            items = obj.ToList();
        }

        public void Register(Action<IDeckModel<DeckItem>> listener)
        {
            _listeners += listener;
        }

        public void Unregister(Action<IDeckModel<DeckItem>> listener)
        {
            _listeners -= listener;
        }

        public void ClearListeners()
        {
            _listeners = null;
        }
    }
}