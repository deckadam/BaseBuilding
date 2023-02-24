using Deck.Agent;
using Deck.Map.Agent.Chest;
using Deck.UI.Health;
using Deck.UI.Inventory;
using Deck.UI.SaveListingMenu;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Deck.Data.Pool
{
    [CreateAssetMenu(fileName = "Deck Binder Pool", menuName = "Deck/Binder/Pool", order = 0)]
    public class DeckBinderPool : ScriptableObjectInstaller
    {
        [SerializeField] private DeckHealthBar healthBarPrefab;
        [SerializeField] private DeckInventoryDisplayerCell inventoryDisplayerCellPrefab;
        [SerializeField] private DeckSaveDisplayer saveDisplayerPrefab;
        [SerializeField] private DeckInventoryPopUp inventoryPopUpPrefab;
        [SerializeField] private DeckAgentCore agentCorePrefab;
        [SerializeField] private DeckConfirmationPopUp confirmationPopUpPrefab;
        [SerializeField] private DeckAgentChest chestAgentPrefab;

        public override void InstallBindings()
        {
            Container.BindFactory<DeckHealthBar, DeckHealthBar.Factory>().FromPoolableMemoryPool<DeckHealthBar, DeckHealthBarPool>(poolBinder => poolBinder.WithInitialSize(5).FromComponentInNewPrefab(healthBarPrefab).UnderTransformGroup("UIPool"));
            Container.BindFactory<DeckInventoryDisplayerCell, DeckInventoryDisplayerCell.Factory>().FromPoolableMemoryPool<DeckInventoryDisplayerCell, DeckInventoryDisplayerCellPool>(poolBinder => poolBinder.WithInitialSize(5).FromComponentInNewPrefab(inventoryDisplayerCellPrefab).UnderTransformGroup("UIPool"));
            Container.BindFactory<DeckSaveDisplayer, DeckSaveDisplayer.Factory>().FromPoolableMemoryPool<DeckSaveDisplayer, DeckSaveDisplayerPool>(poolBinder => poolBinder.WithInitialSize(5).FromComponentInNewPrefab(saveDisplayerPrefab).UnderTransformGroup("UIPool"));
            Container.BindFactory<DeckInventoryPopUp, DeckInventoryPopUp.Factory>().FromPoolableMemoryPool<DeckInventoryPopUp, DeckInventoryPopUpPool>(poolBinder => poolBinder.WithInitialSize(5).FromComponentInNewPrefab(inventoryPopUpPrefab).UnderTransformGroup("UIPool"));
            Container.BindFactory<DeckAgentCore, DeckAgentCore.Factory>().FromPoolableMemoryPool<DeckAgentCore, DeckCoreAgentPool>(poolBinder => poolBinder.WithInitialSize(0).FromComponentInNewPrefab(agentCorePrefab).UnderTransformGroup("Agent"));
            Container.BindFactory<DeckConfirmationPopUp, DeckConfirmationPopUp.Factory>().FromPoolableMemoryPool<DeckConfirmationPopUp, DeckConfirmationPopUpPool>(poolBinder => poolBinder.WithInitialSize(0).FromComponentInNewPrefab(confirmationPopUpPrefab).UnderTransformGroup("UIPool"));
            Container.BindFactory<DeckAgentChest, DeckAgentChest.Factory>().FromComponentInNewPrefab(chestAgentPrefab);
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

        private class DeckCoreAgentPool : MonoPoolableMemoryPool<IMemoryPool, DeckAgentCore>
        {
        }


        private class DeckConfirmationPopUpPool : MonoPoolableMemoryPool<IMemoryPool, DeckConfirmationPopUp>
        {
        }
    }
}