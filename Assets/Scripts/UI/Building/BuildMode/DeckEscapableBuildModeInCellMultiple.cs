using Services.Building;
using Systems.SystemInput.Events;

namespace UI.Building.BuildMode
{
    public class DeckEscapableBuildModeInCellMultiple : DeckEscapableBuildMode
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
            Services.DeckServiceProvider.GetService<DeckServiceBuilding>().BuildInCell(false);
            var rotation = BuildingPage.GetBuildableRotation(Buildable);
            BuildingService.UpdateSilhouetteInCell(rotation, false);
        }

        protected override void InternalOnMouseMove(DeckEventOnMouseMove obj)
        {
            var rotation = BuildingPage.GetBuildableRotation(Buildable);
            BuildingService.UpdateSilhouetteInCell(rotation, false);
        }
    }
}