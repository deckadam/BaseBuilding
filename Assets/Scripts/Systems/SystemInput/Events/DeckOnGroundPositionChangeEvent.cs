using Deck.EventManager;
using UnityEngine;

namespace Deck.InputHandling.Events
{
    public class DeckOnGroundPositionChangeEvent : DeckEvent
    {
        public Vector3 position { get; private set; }

        public static DeckOnGroundPositionChangeEvent Create(Vector3 position)
        {
            return new DeckOnGroundPositionChangeEvent() {position = position};
        }
    }
}