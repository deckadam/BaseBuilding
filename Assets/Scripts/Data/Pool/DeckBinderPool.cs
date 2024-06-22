using Deck.Save.Data;
using Deck.UI.Inventory;
using Deck.UI.Item;
using Deck.UI.SaveListingMenu;
using Deck.UI.Stats;
using Deck.Utility.Health;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Deck.Data.Pool
{
    [CreateAssetMenu(fileName = "Deck Binder Pool", menuName = "Deck/Binder/Pool", order = 0)]
    public class DeckBinderPool : ScriptableObjectInstaller
    {
        [SerializeField] private DeckHealthBar healthBarPrefab;
        [SerializeField] private DeckInstanceCreator instanceCreator;
        [SerializeField] private DeckInventoryDisplayerCell inventoryDisplayerCellPrefab;
        [SerializeField] private DeckSaveDisplayer saveDisplayerPrefab;
        [SerializeField] private DeckInventoryPopUp inventoryPopUpPrefab;
        [SerializeField] private DeckConfirmationPopUp confirmationPopUpPrefab;
        [SerializeField] private DeckUIItemDisplayer itemDisplayer;
        [SerializeField] private DeckStatsPopUp deckStatsPopPrefab;
        [SerializeField] private DeckUIStatElement statElement;
        [SerializeField] private DeckUIStatContainer statContainer;

        public override void InstallBindings()
        {
            Container.BindFactory<DeckHealthBar, DeckHealthBar.Factory>().FromPoolableMemoryPool(poolBinder => poolBinder.WithInitialSize(5).FromComponentInNewPrefab(healthBarPrefab).UnderTransformGroup("UIPool"));
            Container.BindFactory<DeckInventoryDisplayerCell, DeckInventoryDisplayerCell.Factory>().FromPoolableMemoryPool(poolBinder => poolBinder.WithInitialSize(5).FromComponentInNewPrefab(inventoryDisplayerCellPrefab).UnderTransformGroup("UIPool"));
            Container.BindFactory<DeckSaveDisplayer, DeckSaveDisplayer.Factory>().FromPoolableMemoryPool(poolBinder => poolBinder.WithInitialSize(5).FromComponentInNewPrefab(saveDisplayerPrefab).UnderTransformGroup("UIPool"));
            Container.BindFactory<DeckInventoryPopUp, DeckInventoryPopUp.Factory>().FromPoolableMemoryPool(poolBinder => poolBinder.WithInitialSize(5).FromComponentInNewPrefab(inventoryPopUpPrefab).UnderTransformGroup("UIPool"));
            
            //Stats
            Container.BindFactory<DeckStatsPopUp, DeckStatsPopUp.Factory>().FromPoolableMemoryPool(poolBinder => poolBinder.WithInitialSize(5).FromComponentInNewPrefab(deckStatsPopPrefab).UnderTransformGroup("UIPool"));
            Container.BindFactory<DeckUIStatContainer, DeckUIStatContainer.Factory>().FromPoolableMemoryPool(poolBinder => poolBinder.WithInitialSize(5).FromComponentInNewPrefab(statContainer).UnderTransformGroup("UIPool"));
            Container.BindFactory<DeckUIStatElement, DeckUIStatElement.Factory>().FromPoolableMemoryPool(poolBinder => poolBinder.WithInitialSize(5).FromComponentInNewPrefab(statElement).UnderTransformGroup("UIPool"));
            
            Container.BindFactory<DeckConfirmationPopUp, DeckConfirmationPopUp.Factory>().FromPoolableMemoryPool(poolBinder => poolBinder.WithInitialSize(0).FromComponentInNewPrefab(confirmationPopUpPrefab).UnderTransformGroup("UIPool"));
            Container.BindFactory<DeckUIItemDisplayer, DeckUIItemDisplayer.Factory>().FromPoolableMemoryPool(poolBinder => poolBinder.WithInitialSize(5).FromComponentInNewPrefab(itemDisplayer).UnderTransformGroup("UIPool"));
            
            Container.BindInstance(instanceCreator).AsSingle().NonLazy();
            Container.QueueForInject(instanceCreator);
        }
    }
}