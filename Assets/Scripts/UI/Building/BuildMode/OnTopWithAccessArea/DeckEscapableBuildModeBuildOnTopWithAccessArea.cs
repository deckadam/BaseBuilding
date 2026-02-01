namespace UI.Building.BuildMode.OnTopWithAccessArea
{
    public class DeckEscapableBuildModeBuildOnTopWithAccessArea : DeckEscapableBuildMode
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
            BuildingService.BuildWithAccess();
        }

        protected override void InternalOnMouseMove()
        {
            BuildingService.UpdateSilhouetteWithAccess();
        }

        protected override void InternalOnMiddleScroll()
        {
            BuildingService.UpdateSilhouetteWithAccess();
        }
    }
}