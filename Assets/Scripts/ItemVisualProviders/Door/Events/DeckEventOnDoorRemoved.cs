using EventManager;
using UnityEngine;

namespace ItemVisualProviders.Door.Events
{
    public struct DeckEventOnDoorRemoved : IDeckEvent
    {
        public Vector2Int position { get; private set; }

        public static DeckEventOnDoorRemoved Create(Vector2Int position)
        {
            return new DeckEventOnDoorRemoved()
            {
                position = position
            };
        }
    }
}