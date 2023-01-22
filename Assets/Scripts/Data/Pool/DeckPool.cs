using Deck.UI.Health;
using Deck.UI.Inventory;
using Deck.UI.SaveListingMenu;
using UnityEngine;
using Zenject;

namespace Deck.Data.Pool
{
    [CreateAssetMenu(fileName = "Deck Pool", menuName = "Deck/Installer/Pool", order = 0)]
    public class DeckPool : ScriptableObjectInstaller
    {
        [SerializeField] private DeckHealthBar healthBarPrefab;
        [SerializeField] private DeckInventoryDisplayerCell inventoryDisplayerCellPrefab;
        [SerializeField] private DeckSaveDisplayer saveDisplayerPrefab;
        [SerializeField] private DeckInventoryPopUp inventoryPopUpPrefab;

        public override void InstallBindings()
        {
            Container.BindFactory<DeckHealthBar, DeckHealthBar.Factory>().FromPoolableMemoryPool<DeckHealthBar, DeckHealthBarPool>(poolBinder => poolBinder.WithInitialSize(5).FromComponentInNewPrefab(healthBarPrefab).UnderTransformGroup("UIPool"));
            Container.BindFactory<DeckInventoryDisplayerCell, DeckInventoryDisplayerCell.Factory>().FromPoolableMemoryPool<DeckInventoryDisplayerCell, DeckInventoryDisplayerCellPool>(poolBinder => poolBinder.WithInitialSize(5).FromComponentInNewPrefab(inventoryDisplayerCellPrefab).UnderTransformGroup("UIPool"));
            Container.BindFactory<DeckSaveDisplayer, DeckSaveDisplayer.Factory>().FromPoolableMemoryPool<DeckSaveDisplayer, DeckSaveDisplayerPool>(poolBinder => poolBinder.WithInitialSize(5).FromComponentInNewPrefab(saveDisplayerPrefab).UnderTransformGroup("UIPool"));
            Container.BindFactory<DeckInventoryPopUp, DeckInventoryPopUp.Factory>().FromPoolableMemoryPool<DeckInventoryPopUp, DeckInventoryPopUpPool>(poolBinder => poolBinder.WithInitialSize(5).FromComponentInNewPrefab(inventoryPopUpPrefab).UnderTransformGroup("UIPool"));
            Container.BindInstance(this);
        }

        private class DeckHealthBarPool : MonoPoolableMemoryPool<IMemoryPool, DeckHealthBar>
        {
        }

        private class DeckInventoryDisplayerCellPool : MonoPoolableMemoryPool<IMemoryPool, DeckInventoryDisplayerCell>
        {
        }

        private class DeckSaveDisplayerPool : MonoPoolableMemoryPool<IMemoryPool, DeckSaveDisplayer>
        {
        }

        private class DeckInventoryPopUpPool : MonoPoolableMemoryPool<IMemoryPool, DeckInventoryPopUp>
        {
        }
    }
}