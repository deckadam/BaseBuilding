using Services.Building;
using Systems.SystemInput.Events;

namespace UI.Building.BuildMode
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

        protected override void InternalOnLeftClickDown(DeckEventOnLeftClickDown obj)
        {
        }

        protected override void InternalOnLeftClickUp(DeckEventOnLeftClickUp obj)
        {
            Services.DeckServiceProvider.GetService<DeckServiceBuilding>().BuildInCell(_canReplace);
            var rotation = BuildingPage.GetBuildableRotation(Buildable);
            BuildingService.UpdateSilhouetteInCell(rotation, _canReplace);
        }

        protected override void InternalOnMouseMove(DeckEventOnMouseMove obj)
        {
            var rotation = BuildingPage.GetBuildableRotation(Buildable);
            BuildingService.UpdateSilhouetteInCell(rotation, _canReplace);
        }
    }
}