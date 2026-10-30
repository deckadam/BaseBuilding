using EventManager;
using UnityEngine;

namespace Systems.SystemInput.Events
{
    public struct DeckEventOnWaypointPlacementRequested:IDeckEvent
    {
        public Vector3 position { get; private set; }

        public static DeckEventOnWaypointPlacementRequested Create(Vector3 position)
        {
            return new DeckEventOnWaypointPlacementRequested
            {
                position = position
            };
        }

    }
}