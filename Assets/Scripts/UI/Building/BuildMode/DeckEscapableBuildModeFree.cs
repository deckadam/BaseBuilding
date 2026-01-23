using Services;
using Services.Building;

namespace UI.Building.BuildMode
{
    public class DeckEscapableBuildModeFree : DeckEscapableBuildMode
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
            DeckServiceProvider.GetService<DeckServiceBuilding>().BuildFree();
            BuildingService.UpdateSilhouetteFree();
        }

        protected override void InternalOnMouseMove()
        {
            BuildingService.UpdateSilhouetteFree();
        }
    }
}