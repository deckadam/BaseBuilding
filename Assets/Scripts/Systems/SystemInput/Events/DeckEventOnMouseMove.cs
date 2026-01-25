using EventManager;
using UnityEngine;

namespace Systems.SystemInput.Events
{
    public struct DeckEventOnMouseMove : IDeckEvent
    {
        public Vector3 position { get; private set; }
        public Vector2 delta { get; private set; }

        public static DeckEventOnMouseMove Create(Vector3 position, Vector2 delta)
        {
            return new DeckEventOnMouseMove
            {
                position = position,
                delta = delta
            };
        }
    }
}