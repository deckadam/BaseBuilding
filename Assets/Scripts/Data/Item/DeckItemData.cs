using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Deck.Data.Item
{
    [CreateAssetMenu(fileName = "Deck Item Data", menuName = "Deck/Installer/Item", order = 0)]
    public class DeckItemData : ScriptableObjectInstaller
    {
        [SerializeField, InfoBox("Duplicate item exists", nameof(CheckUniqueness))] private List<DeckItem> items;

        private Dictionary<string, DeckItem> _itemsWithNameAccess;

        private bool CheckUniqueness()
        {
            return items.Count != items.Distinct().Count();
        }

        public DeckItem GetItemWithName(string name)
        {
            if (_itemsWithNameAccess.TryGetValue(name, out var item))
            {
                return item;
            }

            throw new Exception("Item with name not found  " + name);
        }

        public IEnumerable<DeckItem> GetItems()
        {
            return items;
        }

        public override void InstallBindings()
        {
            Container.BindInstance(this);

            _itemsWithNameAccess = new Dictionary<string, DeckItem>();
            foreach (var deckItem in items)
            {
                _itemsWithNameAccess[deckItem.name] = deckItem;
            }
        }
    }
}