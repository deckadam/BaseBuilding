using EventManager;
using UnityEngine;

namespace Services.AreaController.Events
{
    public struct DeckEventOnWallBuild : IDeckEvent
    {
        public Vector2Int position { get; private set; }

        public static DeckEventOnWallBuild Create(Vector2Int positions)
        {
            return new DeckEventOnWallBuild()
            {
                position = positions
            };
        }
    }
}