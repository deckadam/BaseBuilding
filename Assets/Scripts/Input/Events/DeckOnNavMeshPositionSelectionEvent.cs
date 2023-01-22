using Deck.EventManager;
using UnityEngine;

namespace Deck.InputHandling.Events
{
    public class DeckOnNavMeshPositionSelectionEvent : DeckEvent
    {
        public Vector3 position { get; private set; }

        public static DeckOnNavMeshPositionSelectionEvent Create(Vector3 pos)
        {
            return new DeckOnNavMeshPositionSelectionEvent() {position = pos};
        }
    }
}