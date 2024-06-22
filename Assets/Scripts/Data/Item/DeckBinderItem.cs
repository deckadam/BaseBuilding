using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Deck.Data.Item
{
    [CreateAssetMenu(fileName = "Deck Binder Item", menuName = "Deck/Binder/Item", order = 0)]
    public class DeckBinderItem : ScriptableObjectInstaller
    {
        [SerializeField] private List<DeckDataItem> items;
        // [SerializeField, InfoBox("Duplicate item exists", nameof(CheckUniqueness))] private List<DeckDataItem> items;

        private Dictionary<string, DeckDataItem> _itemsWithNameAccess;

        private bool CheckUniqueness()
        {
            return items.Count != items.Distinct().Count();
        }

        public DeckDataItem GetItemWithName(string itemName)
        {
            if (_itemsWithNameAccess.TryGetValue(itemName, out var item))
            {
                var newItem = Instantiate(item);
                newItem.ResetAmount();
                return newItem;
            }

            throw new Exception("Item with name not found  " + itemName);
        }

        public List<DeckDataItem> GetItems()
        {
            return items;
        }

        public override void InstallBindings()
        {
            Container.BindInstance(this);

            _itemsWithNameAccess = new Dictionary<string, DeckDataItem>();
            foreach (var deckItem in items)
            {
                _itemsWithNameAccess[deckItem.Name] = deckItem;
            }
        }
    }
}