using Deck.Save.Data;
using Deck.Components.Building.Inventory;
using Deck.Components.Building.Item;
using Deck.Components.Building.SaveListingMenu;
using Deck.Components.Building.Stats;
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
        [FormerlySerializedAs("instanceCreator")] [SerializeField] private DeckInstanceProvider instanceProvider;
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
            Container.BindInstance(instanceProvider).AsSingle().NonLazy();
            Container.QueueForInject(instanceProvider);
        }
    }
}