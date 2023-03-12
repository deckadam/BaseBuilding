using Cinemachine;
using Deck.Agent;
using Deck.Components;
using Deck.Inventory;
using Deck.Map.Agent.Chest;
using Deck.Map.Selection;
using Deck.Save;
using Deck.Services.Implementations;
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
            //Component binds
            Container.Bind<DeckComponent>().To<DeckMovementComponent>().AsTransient().WhenInjectedInto<DeckAgentCore>();
            Container.Bind<DeckComponent>().To<DeckHealthComponent>().AsTransient().WhenInjectedInto<DeckAgentCore>();
            Container.Bind<DeckComponent>().To<DeckComponentDamageDealer>().AsTransient().WhenInjectedInto<DeckAgentCore>();
            Container.Bind<DeckComponent>().To<DeckInventoryComponent>().AsTransient().WhenInjectedInto<DeckAgentCore>();

            Container.Bind<DeckComponent>().To<DeckHealthComponent>().AsTransient().WhenInjectedInto<DeckAgentChest>();
            Container.Bind<DeckComponent>().To<DeckInventoryComponent>().AsTransient().WhenInjectedInto<DeckAgentChest>();
            Container.Bind<DeckComponent>().To<DeckMovementComponent>().AsTransient().WhenInjectedInto<DeckAgentChest>();

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