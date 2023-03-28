using Cinemachine;
using Deck.Components;
using Deck.Events;
using Deck.Save;
using Deck.UI.Health;
using Deck.UI.Inventory;
using UnityEngine;
using Zenject;

namespace Deck.Installers
{
    public class DeckBinderClass : MonoInstaller
    {
        public override void InstallBindings()
        {
            //Core agent
            Container.Bind<DeckComponent>().To<DeckComponentMovement>().AsTransient().WhenInjectedInto<DeckAgentCore>();
            Container.Bind<DeckComponent>().To<DeckComponentHealth>().AsTransient().WhenInjectedInto<DeckAgentCore>();
            Container.Bind<DeckComponent>().To<DeckComponentDamageDealer>().AsTransient().WhenInjectedInto<DeckAgentCore>();
            Container.Bind<DeckComponent>().To<DeckComponentInventory>().AsTransient().WhenInjectedInto<DeckAgentCore>();
            Container.Bind<DeckComponent>().To<DeckComponentAnimatorCore>().AsTransient().WhenInjectedInto<DeckAgentCore>();
            Container.Bind<DeckComponent>().To<DeckComponentItemHolder>().AsTransient().WhenInjectedInto<DeckAgentCore>();

            //Chest agent
            Container.Bind<DeckComponent>().To<DeckComponentHealth>().AsTransient().WhenInjectedInto<DeckAgentChest>();
            Container.Bind<DeckComponent>().To<DeckComponentInventory>().AsTransient().WhenInjectedInto<DeckAgentChest>();
            Container.Bind<DeckComponent>().To<DeckComponentMovement>().AsTransient().WhenInjectedInto<DeckAgentChest>();
            Container.Bind<DeckComponent>().To<DeckComponentAnimatorChest>().AsTransient().WhenInjectedInto<DeckAgentChest>();

            //Tree agent
            Container.Bind<DeckComponent>().To<DeckComponentHealth>().AsTransient().WhenInjectedInto<DeckAgentHarvestable>();
            Container.Bind<DeckComponent>().To<DeckComponentItemDropper>().AsTransient().WhenInjectedInto<DeckAgentHarvestable>();
            Container.Bind<DeckComponent>().To<DeckComponentAnimatorTree>().AsTransient().WhenInjectedInto<DeckAgentHarvestable>();

            //Wall agent
            Container.Bind<DeckComponent>().To<DeckComponentHealth>().AsTransient().WhenInjectedInto<DeckAgentWall>();
            Container.Bind<DeckComponent>().To<DeckComponentMovement>().AsTransient().WhenInjectedInto<DeckAgentWall>();

            //Scene references
            Container.Bind<CinemachineConfiner>().FromComponentInHierarchy().AsSingle();
            Container.Bind<CinemachineVirtualCamera>().FromComponentInHierarchy().AsSingle();
            Container.Bind<DeckInventoryPopUp>().FromComponentInHierarchy().AsCached();
            Container.Bind<DeckUIWorldLabelDisplayer>().FromComponentInHierarchy().AsCached();
            Container.Bind<Camera>().FromComponentInHierarchy().AsCached();
            Container.Bind<CinemachineBrain>().FromComponentInHierarchy().AsCached();

            //Class references
            Container.Bind<DeckFactoryProviderUI>().AsSingle();
            Container.Bind<DeckAgentLoadResolver>().AsSingle();
        }
    }
}