using Services.Building;
using Systems.SystemInput.Events;

namespace UI.Building.BuildMode
{
    public class DeckEscapableBuildModeOnTop : DeckEscapableBuildMode
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
            Services.DeckServiceProvider.GetService<DeckServiceBuilding>().BuildOnTop();
            BuildingService.UpdateSilhouetteOnTop();
        }

        protected override void InternalOnMouseMove(DeckEventOnMouseMove obj)
        {
            BuildingService.UpdateSilhouetteOnTop();
        }
    }
}