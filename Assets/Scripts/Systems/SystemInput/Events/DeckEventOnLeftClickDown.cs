using Deck.EventManager;
using UnityEngine;

namespace Deck.InputHandling.Events
{
    public class DeckEventOnLeftClickDown : IDeckEvent
    {
        public Vector3 position { get; private set; }

        public static DeckEventOnLeftClickDown Create(Vector3 position)
        {
            return new DeckEventOnLeftClickDown
            {
                position = position
            };
        }
    }
}