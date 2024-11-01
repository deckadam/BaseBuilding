using Deck.EventManager;
using UnityEngine;

namespace Deck.InputHandling.Events
{
    public class DeckEventOnLeftClickUp : DeckEvent
    {
        public bool movedOverToUI { get; private set; }
        public Vector3 position { get; private set; }

        public static DeckEventOnLeftClickUp Create(Vector3 position,bool movedOverToUI)
        {
            return new DeckEventOnLeftClickUp
            {
                position = position,
                movedOverToUI = movedOverToUI
            };
        }
    }
}