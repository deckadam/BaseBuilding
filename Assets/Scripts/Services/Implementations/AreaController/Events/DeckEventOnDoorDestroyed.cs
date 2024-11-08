using Deck.EventManager;
using UnityEngine;

namespace Deck.Services.Implementations.AreaController.Events
{
    public struct DeckEventOnDoorDestroyed : IDeckEvent
    {
        public Vector2Int position { get; private set; }

        public static DeckEventOnDoorDestroyed Create(Vector2Int position)
        {
            return new DeckEventOnDoorDestroyed()
            {
                position = position
            };
        }
    }
}