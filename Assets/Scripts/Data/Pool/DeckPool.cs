using Deck.UI.Health;
using Deck.UI.Inventory;
using Sirenix.Serialization;
using UnityEngine;
using Zenject;

namespace Deck.Data.Pool
{
    [CreateAssetMenu(fileName = "Deck Pool", menuName = "Deck/Installer/Pool", order = 0)]
    public class DeckPool : ScriptableObjectInstaller
    {
        [SerializeField] private DeckHealthBar healthBar;
        [SerializeField] private DeckInventoryDisplayerCell inventoryDisplayerCell;

        public override void InstallBindings()
        {
            Container.BindFactory<DeckHealthBar, DeckHealthBar.Factory>().FromPoolableMemoryPool<DeckHealthBar, DeckHealthBarPool>(poolBinder => poolBinder.WithInitialSize(5).FromComponentInNewPrefab(healthBar).UnderTransformGroup("HealthBars"));
            Container.BindFactory<DeckInventoryDisplayerCell, DeckInventoryDisplayerCell.Factory>().FromPoolableMemoryPool<DeckInventoryDisplayerCell, DeckInventoryDisplayerCellPool>(poolBinder => poolBinder.WithInitialSize(5).FromComponentInNewPrefab(inventoryDisplayerCell).UnderTransformGroup("InventoryCells"));
            Container.BindInstance(this);
        }

        private class DeckHealthBarPool : MonoPoolableMemoryPool<IMemoryPool, DeckHealthBar>
        {
        }

        private class DeckInventoryDisplayerCellPool : MonoPoolableMemoryPool<IMemoryPool, DeckInventoryDisplayerCell>
        {
        }
    }
}