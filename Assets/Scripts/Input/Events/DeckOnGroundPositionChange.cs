using Deck.EventManager;
using UnityEngine;

namespace Deck.InputHandling.Events
{
    public class DeckOnGroundPositionChange : DeckEvent
    {
        public Vector3 position { get; private set; }

        public static DeckOnGroundPositionChange Create(Vector3 position)
        {
            return new DeckOnGroundPositionChange() {position = position};
        }
    }
}