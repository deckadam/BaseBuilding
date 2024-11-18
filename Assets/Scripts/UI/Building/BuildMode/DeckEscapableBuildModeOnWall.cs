using Deck.InputHandling.Events;
using Deck.Services.Building;

namespace Deck.UI.Building.BuildMode
{
    public class DeckEscapableBuildModeOnWall : DeckEscapableBuildMode
    {
        protected override void InternalOnInitialize()
        {
            BuildingService.StartSilhouette(Buildable);
        }

        protected override void InternalOnLeftClickDown(DeckEventOnLeftClickDown obj)
        {
        }

        protected override void InternalOnLeftClickUp(DeckEventOnLeftClickUp obj)
        {
            Deck.GetService<DeckServiceBuilding>().BuildOnWall();
            BuildingService.UpdateSilhouetteOnWall();
        }

        protected override void InternalOnMouseMove(DeckEventOnMouseMove obj)
        {
            BuildingService.UpdateSilhouetteOnWall();
        }
    }
}