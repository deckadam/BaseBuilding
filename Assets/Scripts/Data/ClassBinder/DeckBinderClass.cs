using Cinemachine;
using Deck.Components;
using Deck.Inventory;
using Deck.Map.Selection;
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
            Container.Bind<DeckMovementComponent>().AsTransient();
            Container.Bind<DeckHealthComponent>().AsTransient();
            Container.Bind<DeckComponentDamageDealer>().AsTransient();
            Container.Bind<DeckInventoryComponent>().AsTransient();


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