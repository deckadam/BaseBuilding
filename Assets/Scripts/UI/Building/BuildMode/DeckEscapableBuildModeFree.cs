using Deck.InputHandling.Events;
using Deck.Services.Building;

namespace Deck.UI.Building.BuildMode
{
    public class DeckEscapableBuildModeFree : DeckEscapableBuildMode
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
            Deck.GetService<DeckServiceBuilding>().BuildFree();
            BuildingService.UpdateSilhouetteFree();
        }

        protected override void InternalOnMouseMove(DeckEventOnMouseMove obj)
        {
            BuildingService.UpdateSilhouetteFree();
        }
    }
}