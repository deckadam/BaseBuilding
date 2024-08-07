using Deck.ItemVisualProviders;
using Deck.Services.Building;
using Deck.Utility;
using UnityEngine;
using Zenject;

namespace Deck.UI.InGame.Agent.Door
{
    public class DeckAgentDoor : DeckBuilding
    {
        private DeckItemVisualProviderDoor _doorProvider;

        [Inject]
        private void Inject(DeckItemVisualProviderDoor doorProvider)
        {
            _doorProvider = doorProvider;
        }

        protected override void InternalRequestDeath()
        {
            Deck.GetService<DeckServiceBuilding>().OnBuildingDestroyed(this);
            _doorProvider.ReturnItemVisual(transform.position.ToVector2Int());
        }
    }
}