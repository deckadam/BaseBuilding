using EventManager;
using UnityEngine;

namespace Systems.SystemInput.Events
{
    public struct DeckEventOnAxisMovement : IDeckEvent
    {
        public Vector2 movement { get; private set; }

        public static DeckEventOnAxisMovement Create(Vector2 movement)
        {
            return new DeckEventOnAxisMovement()
            {
                movement = movement
            };
        }
    }
}