using Systems.SystemInput.Events;

namespace UI.Building.BuildMode
{
    public class DeckEscapableBuildModeBuildOnTopWithAccessArea : DeckEscapableBuildMode
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
            BuildingService.BuildWithAccess();
        }

        protected override void InternalOnMouseMove(DeckEventOnMouseMove obj)
        {
            BuildingService.UpdateSilhouetteWithAccess();
        }
    }
}