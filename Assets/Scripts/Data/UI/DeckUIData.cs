using Deck.Inventory.UI;
using UnityEngine;
using Zenject;

namespace Deck.Data.UI
{
    [CreateAssetMenu(fileName = "Deck UI Data", menuName = "Deck/Installer/UI", order = 0)]
    public class DeckUIData : ScriptableObjectInstaller
    {
        public DeckInventoryDisplayerCell cellPrefab;

        public override void InstallBindings()
        {
            Container.BindInstance(this);
        }
    }
}