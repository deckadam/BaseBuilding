using Services;
using Services.Building;

namespace UI.Building.BuildMode
{
    public class DeckEscapableBuildModeInCellMultiple : DeckEscapableBuildMode
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
            DeckServiceProvider.GetService<DeckServiceBuilding>().BuildInCell(false);
            var rotation = BuildingPage.GetBuildableRotation(Buildable);
            BuildingService.UpdateSilhouetteInCell(rotation, false);
        }

        protected override void InternalOnMouseMove()
        {
            var rotation = BuildingPage.GetBuildableRotation(Buildable);
            BuildingService.UpdateSilhouetteInCell(rotation, false);
        }

        protected override void InternalOnBuildModeCanceled()
        {
            BuildingService.ClearAll();
        }
    }
}