using Deck.EventManager;
using UnityEngine;

namespace Deck.InputHandling.Events
{
    public class DeckEventOnLeftClick : DeckEvent
    {
        public Vector3 position { get; private set; }

        public static DeckEventOnLeftClick Create(Vector3 position)
        {
            return new()
            {
                position = position
            };
        }
    }
}