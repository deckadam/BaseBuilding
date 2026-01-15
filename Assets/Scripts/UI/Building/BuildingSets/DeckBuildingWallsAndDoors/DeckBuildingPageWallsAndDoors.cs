using Data.Buildable;
using Deck.Services.Cam;
using UnityEngine;

namespace UI.Building.BuildingSets.DeckBuildingWallsAndDoors
{
    public class DeckBuildingPageWallsAndDoors : DeckBuildingPage
    {
        public DeckBuildable doorBuildable;

        private readonly Quaternion horizontalDoorRotation = Quaternion.Euler(0, 90, 0);

        private DeckServiceCamera _serviceCamera;

        protected override void InternalInitialize()
        {
            _serviceCamera = Services.DeckServiceProvider.GetService<DeckServiceCamera>();
        }

        public override Quaternion GetBuildableRotation(DeckBuildable buildable)
        {
            if (buildable.Equals(doorBuildable))
            {
                return Quaternion.identity;
            }

            var cursorCellIndex = _serviceCamera.GetCursorCellIndex();
            var neighbourStatus = BuildingService.GetCellNeighbourStatus(cursorCellIndex);

            if (neighbourStatus[0] && neighbourStatus[1])
            {
                return horizontalDoorRotation;
            }

            if (neighbourStatus[2] && neighbourStatus[3])
            {
                return Quaternion.identity;
            }

            if (neighbourStatus[0] || neighbourStatus[1])
            {
                return horizontalDoorRotation;
            }

            return Quaternion.identity;
        }
    }
}