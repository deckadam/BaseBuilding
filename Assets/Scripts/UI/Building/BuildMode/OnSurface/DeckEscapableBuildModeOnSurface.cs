using Services.Building;

namespace UI.Building.BuildMode.OnSurface
{
    public class DeckEscapableBuildModeOnSurface : DeckEscapableBuildMode
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

        protected override void InternalOnMiddleScroll()
        {
            
        }
    }
}