using Deck.EventManager;
using UnityEngine;

namespace Deck.InputHandling.Events
{
    public class DeckOnNavMeshPositionSelection : DeckEvent
    {
        public Vector3 position { get; private set; }

        public static DeckOnNavMeshPositionSelection Create(Vector3 pos)
        {
            return new DeckOnNavMeshPositionSelection() {position = pos};
        }
    }
}