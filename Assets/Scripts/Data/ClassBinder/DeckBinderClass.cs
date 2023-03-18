using Cinemachine;
using Deck.Agent;
using Deck.Components;
using Deck.Map.Agent.Chest;
using Deck.Map.Selection;
using Deck.Save;
using Deck.Services.Implementations;
using Deck.Utility.Constants.Health;
using Deck.Utility.Constants.Inventory;
using UnityEngine;
using Zenject;

namespace Deck.Installers
{
    public class DeckBinderClass : MonoInstaller
    {
        public override void InstallBindings()
        {
            //Component binds
            Container.Bind<DeckComponent>().To<DeckComponentMovement>().AsTransient().WhenInjectedInto<DeckAgentCore>();
            Container.Bind<DeckComponent>().To<DeckComponentHealth>().AsTransient().WhenInjectedInto<DeckAgentCore>();
            Container.Bind<DeckComponent>().To<DeckComponentDamageDealer>().AsTransient().WhenInjectedInto<DeckAgentCore>();
            Container.Bind<DeckComponent>().To<DeckComponentInventory>().AsTransient().WhenInjectedInto<DeckAgentCore>();

            Container.Bind<DeckComponent>().To<DeckComponentHealth>().AsTransient().WhenInjectedInto<DeckAgentChest>();
            Container.Bind<DeckComponent>().To<DeckComponentInventory>().AsTransient().WhenInjectedInto<DeckAgentChest>();
            Container.Bind<DeckComponent>().To<DeckComponentMovement>().AsTransient().WhenInjectedInto<DeckAgentChest>();
            Container.Bind<DeckComponent>().To<DeckComponentAnimatorChest>().AsTransient().WhenInjectedInto<DeckAgentChest>();

            //Scene references
            Container.Bind<CinemachineConfiner>().FromComponentInHierarchy().AsSingle();
            Container.Bind<CinemachineVirtualCamera>().FromComponentInHierarchy().AsSingle();
            Container.Bind<DeckInventoryPopUp>().FromComponentInHierarchy().AsCached();
            Container.Bind<DeckHealthUI>().FromComponentInHierarchy().AsCached();
            Container.Bind<Camera>().FromComponentInHierarchy().AsCached();
            Container.Bind<DeckSelectionHighlighter>().FromComponentInHierarchy().AsCached();
            Container.Bind<CinemachineBrain>().FromComponentInHierarchy().AsCached();

            //Class references
            Container.Bind<DeckPopUpFactoryProvider>().AsSingle();
            Container.Bind<DeckAgentLoadResolver>().AsSingle();
        }
    }
}