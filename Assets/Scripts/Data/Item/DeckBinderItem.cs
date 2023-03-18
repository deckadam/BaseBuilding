using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Deck.Data.Item
{
    [CreateAssetMenu(fileName = "Deck Binder Item", menuName = "Deck/Binder/Item", order = 0)]
    public class DeckBinderItem : ScriptableObjectInstaller
    {
        [SerializeField, InfoBox("Duplicate item exists", nameof(CheckUniqueness))] private List<DeckDataItem> items;

        private Dictionary<string, DeckDataItem> _itemsWithNameAccess;

        private bool CheckUniqueness()
        {
            return items.Count != items.Distinct().Count();
        }

        public DeckDataItem GetItemWithName(string name)
        {
            if (_itemsWithNameAccess.TryGetValue(name, out var item))
            {
                var newItem = Instantiate(item);
                newItem.ResetAmount();
                newItem.AddToAmount(item.GetAmount());
                return newItem;
            }

            throw new Exception("Item with name not found  " + name);
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
                _itemsWithNameAccess[deckItem.name] = deckItem;
            }
        }
    }
}