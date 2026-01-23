using Services.Building;
using Systems.SystemInput.Events;

namespace UI.Building.BuildMode
{
    public class DeckEscapableBuildModeOnWall : DeckEscapableBuildMode
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
            Services.DeckServiceProvider.GetService<DeckServiceBuilding>().BuildOnWall();
            BuildingService.UpdateSilhouetteOnWall();
        }

        protected override void InternalOnMouseMove()
        {
            BuildingService.UpdateSilhouetteOnWall();
        }

        protected override void InternalOnBuildModeCanceled()
        {
            BuildingService.ClearAll();
        }
    }
}