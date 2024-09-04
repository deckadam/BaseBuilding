using Deck.Components.Building;
using Deck.ItemVisualProviders;
using Deck.Services.Building;
using Deck.Utility;
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

        protected override void InternalRequestDestroy()
        {
            Deck.GetService<DeckServiceBuilding>().OnBuildingDestroyed(this);
            _doorProvider.ReturnItemVisual(transform.position.ToVector2Int(), false);
        }
    }
}