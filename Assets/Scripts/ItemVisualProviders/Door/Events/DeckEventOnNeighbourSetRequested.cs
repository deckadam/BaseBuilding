using EventManager;
using UnityEngine;

namespace ItemVisualProviders.Door.Events
{
    public struct DeckEventOnNeighbourSetRequested : IDeckEvent
    {
        public Vector2Int position { get; private set; }
        public bool[] neighbours { get; private set; }

        public static DeckEventOnNeighbourSetRequested Create(Vector2Int position, ref bool[] neighbours)
        {
            return new DeckEventOnNeighbourSetRequested()
            {
                position = position,
                neighbours = neighbours,
            };
        }
    }
}