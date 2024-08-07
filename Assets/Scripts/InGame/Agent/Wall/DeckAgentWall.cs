using Deck.ItemVisualProviders;
using Deck.Services.Building;
using Deck.UI;
using Deck.Utility;
using Zenject;

namespace Deck.Agent
{
    public class DeckAgentWall : DeckBuilding
    {
        private DeckItemVisualProviderWall _wallProvider;

        [Inject]
        private void Inject(DeckItemVisualProviderWall wallProvider)
        {
            _wallProvider = wallProvider;
        }

        protected override void InternalRequestDeath()
        {
            Deck.GetService<DeckServiceBuilding>().OnBuildingDestroyed(this);
            _wallProvider.ReturnItemVisual(transform.position.ToVector2Int());
        }
    }
}