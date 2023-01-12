using Cinemachine;
using Deck.Components;
using Deck.Inventory;
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
            Container.Bind<DeckMovementComponent>().AsTransient();
            Container.Bind<DeckHealthComponent>().AsTransient();
            Container.Bind<DeckDamageDealerComponent>().AsTransient();
            
            Container.Bind<CinemachineConfiner>().FromComponentInHierarchy().AsSingle();
            Container.Bind<CinemachineVirtualCamera>().FromComponentInHierarchy().AsSingle();
            Container.Bind<DeckInventory>().FromNewComponentSibling();
            Container.Bind<DeckInventoryUI>().FromComponentInHierarchy().AsCached();
            Container.Bind<DeckHealthUI>().FromComponentInHierarchy().AsCached();
            Container.Bind<Camera>().FromComponentInHierarchy().AsCached();
        }
    }
}