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

        protected override void InternalOnLeftClickDown()
        {
        }

        protected override void InternalOnLeftClickUp()
        {
            Services.DeckServiceProvider.GetService<DeckServiceBuilding>().BuildOnTop();
            BuildingService.UpdateSilhouetteOnTop();
        }

        protected override void InternalOnMouseMove()
        {
            BuildingService.UpdateSilhouetteOnTop();
        }

        protected override void InternalOnBuildModeCanceled()
        {
            BuildingService.ClearAll();
        }
    }
}