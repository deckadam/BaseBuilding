using System.Collections.Generic;
using Deck.Inventory.Item;
using UnityEngine;

namespace Deck.Inventory.Inventory
{
    public class DeckInventory : MonoBehaviour
    {
        [SerializeField] private List<DeckItem> items = new();

        public void AddItem(DeckItem deckItemToAdd)
        {
            items.Add(deckItemToAdd);
        }

        public void RemoveItem(DeckItem deckItemToRemove)
        {
            items.Remove(deckItemToRemove);
        }

        public IEnumerable<DeckItem> GetItems()
        {
            return items;
        }
    }
}