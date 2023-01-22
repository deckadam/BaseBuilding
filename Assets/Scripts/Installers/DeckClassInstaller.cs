using Cinemachine;
using Deck.Components;
using Deck.Inventory;
using Deck.Services.Implementations;
using Deck.UI.Health;
using Deck.UI.Inventory;
using UnityEngine;
using Zenject;

namespace Deck.Installers
{
    public class DeckClassInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            //Component references
            Container.Bind<IDeckComponent>().To<DeckMovementComponent>().AsTransient();
            Container.Bind<IDeckComponent>().To<DeckHealthComponent>().AsTransient();
            Container.Bind<IDeckComponent>().To<DeckDamageDealerComponent>().AsTransient();
            Container.Bind<IDeckComponent>().To<DeckInventoryComponent>().AsTransient();


            //Scene references
            Container.Bind<CinemachineConfiner>().FromComponentInHierarchy().AsSingle();
            Container.Bind<CinemachineVirtualCamera>().FromComponentInHierarchy().AsSingle();
            Container.Bind<DeckInventoryPopUp>().FromComponentInHierarchy().AsCached();
            Container.Bind<DeckHealthUI>().FromComponentInHierarchy().AsCached();
            Container.Bind<Camera>().FromComponentInHierarchy().AsCached();

            //Class references
            Container.Bind<DeckPopUpFactoryProvider>().AsSingle();
        }
    }
}