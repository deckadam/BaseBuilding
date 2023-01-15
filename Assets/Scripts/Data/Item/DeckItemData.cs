using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

namespace Deck.Data.Item
{
    [CreateAssetMenu(fileName = "Deck Item Data", menuName = "Deck/Installer/Item", order = 0)]
    public class DeckItemData : ScriptableObjectInstaller
    {
        [SerializeField] private List<DeckItem> items;

        public IEnumerable<DeckItem> GetItems()
        {
            return items;
        }

        public override void InstallBindings()
        {
            Container.BindInstance(this);
        }
    }
}