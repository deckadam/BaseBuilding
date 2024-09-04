using Deck.InGame.Agent.Building;
using Deck.ItemVisualProviders;
using Deck.Services.Building;
using Deck.Utility;
using UnityEngine;
using Zenject;

namespace Deck.InGame.Agent.Wall
{
    public class DeckAgentWall : DeckBuilding
    {
        private DeckItemVisualProviderWall _wallProvider;

        [Inject]
        private void Inject(DeckItemVisualProviderWall wallProvider)
        {
            _wallProvider = wallProvider;
        }

        protected override void InternalRequestDestroy()
        {
            Deck.GetService<DeckServiceBuilding>().OnBuildingDestroyed(this);
            _wallProvider.ReturnItemVisual(transform.position.ToVector2Int(), true);
        }
    }
}