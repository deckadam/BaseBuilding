using Cysharp.Threading.Tasks;
using Deck.Components.Building;
using Deck.ItemVisualProviders;
using Deck.Services.Implementations.AreaController.Events;
using Deck.Utility;
using Deck.Utility.Logger;
using Zenject;

namespace Deck.Components.Wall
{
    public class DeckAgentWall : DeckBuilding
    {
        private DeckItemVisualProviderWall _wallProvider;

        [Inject]
        private void Inject(DeckItemVisualProviderWall wallProvider)
        {
            _wallProvider = wallProvider;
        }

        protected override async void AfterInitialize()
        {
            await UniTask.Yield();
            DeckEventOnWallBuild.Create(transform.position.ToVector2Int()).Send();
        }

        protected override void OnAgentDestroyed()
        {
            DeckEventOnWallDestroyed.Create(transform.position.ToVector2Int()).Send();
        }

        protected override void OnBuildingDestroyed()
        {
            _wallProvider.ReturnItemVisual(transform.position.ToVector2Int(), true);
        }
    }
}