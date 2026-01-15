using EventManager;
using UnityEngine;

namespace Systems.SystemInput.Events
{
    public class DeckEventOnLeftClickUp : IDeckEvent
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