using Services;
using Services.Building;

namespace UI.Building.BuildMode.InCell
{
    public class DeckEscapableBuildModeInCell : DeckEscapableBuildMode
    {
        private bool _canReplace;

        public DeckEscapableBuildModeInCell(bool canReplace)
        {
            _canReplace = canReplace;
        }

        protected override void InternalOnInitialize()
        {
            BuildingService.StartSilhouette(Buildable);
        }

        protected override void InternalOnLeftClickDown()
        {
        }

        protected override void InternalOnLeftClickUp()
        {
            DeckServiceProvider.GetService<DeckServiceBuilding>().BuildInCell(_canReplace);
            var rotation = BuildingPage.GetBuildableRotation(Buildable);
            BuildingService.UpdateSilhouetteInCell(rotation, _canReplace);
        }

        protected override void InternalOnMouseMove()
        {
            var rotation = BuildingPage.GetBuildableRotation(Buildable);
            BuildingService.UpdateSilhouetteInCell(rotation, _canReplace);
        }

        protected override void InternalOnMiddleScroll()
        {
            
        }
    }
}