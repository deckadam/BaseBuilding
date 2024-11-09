using Cysharp.Threading.Tasks;
using Deck.Components.Building;
using Deck.ItemVisualProviders;
using Deck.Services.Implementations.AreaController.Events;
using Deck.Utility;
using Deck.Utility.Logger;
using Zenject;

namespace Deck.Components.Door
{
    public class DeckAgentDoor : DeckBuilding
    {
        private DeckItemVisualProviderDoor _doorProvider;

        [Inject]
        private void Inject(DeckItemVisualProviderDoor doorProvider)
        {
            _doorProvider = doorProvider;
        }

        protected override async void AfterInitialize()
        {
            await UniTask.Yield();

            DeckEventOnDoorBuild.Create(transform.position.ToVector2Int()).Send();
        }

        protected override void OnBuildingDestroyed()
        {
            var cellPosition = transform.position.ToVector2Int();
            _doorProvider.ReturnItemVisual(cellPosition, false);
            DeckEventOnDoorDestroyed.Create(cellPosition).Send();
        }
    }
}