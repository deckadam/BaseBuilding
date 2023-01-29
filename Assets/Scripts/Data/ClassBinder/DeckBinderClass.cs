using Cinemachine;
using Deck.Components;
using Deck.Inventory;
using Deck.Map.Selection;
using Deck.Services.Implementations;
using Deck.Services.Implementations.CellSelectionService;
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
            //Component references
            Container.Bind<DeckComponent>().To<DeckMovementComponent>().AsTransient();
            Container.Bind<DeckComponent>().To<DeckHealthComponent>().AsTransient();
            Container.Bind<DeckComponent>().To<DeckDamageDealerComponent>().AsTransient();
            Container.Bind<DeckComponent>().To<DeckInventoryComponent>().AsTransient();


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
        }
    }
}