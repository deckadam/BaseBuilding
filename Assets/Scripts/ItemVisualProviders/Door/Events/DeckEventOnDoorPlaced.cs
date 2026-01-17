using EventManager;
using UnityEngine;

namespace ItemVisualProviders.Door.Events
{
    public struct DeckEventOnDoorPlaced : IDeckEvent
    {
        public Vector2Int position { get; private set; }

        public static DeckEventOnDoorPlaced Create(Vector2Int position)
        {
            return new DeckEventOnDoorPlaced()
            {
                position = position
            };
        }
    }
}