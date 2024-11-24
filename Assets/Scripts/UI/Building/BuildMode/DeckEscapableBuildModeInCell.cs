using Deck.InputHandling.Events;
using Deck.Services.Building;
using UnityEngine;

namespace Deck.UI.Building.BuildMode
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
            Deck.GetService<DeckServiceBuilding>().BuildInCell(_canReplace);
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