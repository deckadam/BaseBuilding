using Deck.InputHandling.Events;
using Deck.Services.Building;
using UnityEngine;

namespace Deck.UI.Building.BuildMode
{
    public class DeckEscapableBuildModeInCell : DeckEscapableBuildMode
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
            Deck.GetService<DeckServiceBuilding>().BuildInCell();
            BuildingService.UpdateSilhouetteInCell(Quaternion.identity);
        }

        protected override void InternalOnMouseMove(DeckEventOnMouseMove obj)
        {
            BuildingService.UpdateSilhouetteInCell(Quaternion.identity);
        }
    }
}