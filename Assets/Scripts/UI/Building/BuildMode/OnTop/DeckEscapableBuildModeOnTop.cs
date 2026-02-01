using Services.Building;

namespace UI.Building.BuildMode.OnTop
{
    public class DeckEscapableBuildModeOnTop : DeckEscapableBuildMode
    {
        protected override void InternalOnInitialize()
        {
            BuildingService.StartSilhouette(Buildable);
            BuildingService.UpdateSilhouetteOnTop();
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

        protected override void InternalOnMiddleScroll()
        {
            BuildingService.UpdateSilhouetteOnTop();
        }
    }
}