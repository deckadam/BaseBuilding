using Deck.Components.Building;
using Deck.ItemVisualProviders;
using Deck.Utility;
using Deck.Utility.Logger;
using Deck.Services.Building;
using Services.Implementations.AreaController.Events;
using UnityEngine;
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

        protected override void OnAgentDestroyed()
        {
            Debug.LogError("Wall destroyed");
            DeckEventOnWallDestroyed.Create(transform.position.ToVector2Int()).Send();
        }

        protected override void InternalRequestDestroy()
        {
            Deck.GetService<DeckServiceBuilding>().OnBuildingDestroyed(this);
            _wallProvider.ReturnItemVisual(transform.position.ToVector2Int(), true);
        }
    }
}