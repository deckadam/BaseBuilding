using Cinemachine;
using Deck.Inventory.Inventory;
using Deck.Inventory.UI;
using Deck.Player;
using Zenject;

namespace Deck.Installers
{
    public class DeckClassInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IDeckCorePlayerSystem>().To<DeckMovementHandler>().AsTransient();
            Container.Bind<CinemachineConfiner>().FromComponentInHierarchy().AsSingle();
            Container.Bind<CinemachineVirtualCamera>().FromComponentInHierarchy().AsSingle();
            Container.Bind<DeckInventory>().FromNewComponentSibling();
            Container.Bind<DeckInventoryDisplayer>().FromComponentInHierarchy();
        }
    }
}