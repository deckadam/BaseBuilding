using Deck.EventManager;
using UnityEngine;

namespace Deck.InputHandling.Events
{
    public class DeckEventOnMouseMove : IDeckEvent
    {
        public Vector3 position { get; private set; }

        public static DeckEventOnMouseMove Create(Vector3 position)
        {
            return new DeckEventOnMouseMove
            {
                position = position
            };
        }
    }
}