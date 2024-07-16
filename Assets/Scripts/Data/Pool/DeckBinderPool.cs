using Deck.Save.Data;
using Deck.UI.Inventory;
using Deck.UI.Item;
using Deck.UI.SaveListingMenu;
using Deck.UI.Stats;
using Deck.Utility.Health;
using UnityEngine;
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
            Container.BindInstance(instanceCreator).AsSingle().NonLazy();
            Container.QueueForInject(instanceCreator);
        }
    }
}