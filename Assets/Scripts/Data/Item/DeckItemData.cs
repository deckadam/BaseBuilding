using Deck.Data.Item;
using UnityEngine;
using Zenject;

namespace Deck.Data.Item
{
    [CreateAssetMenu(fileName = "Deck Item Data", menuName = "Deck/Installer/Item", order = 0)]
    public class DeckItemData : ScriptableObjectInstaller
    {
        public DeckItem[] items;

        public override void InstallBindings()
        {
            Container.BindInstance(this);
        }
    }
}