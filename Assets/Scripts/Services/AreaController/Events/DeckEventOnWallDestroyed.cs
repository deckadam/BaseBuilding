using Deck.EventManager;
using UnityEngine;

namespace Deck.Services.AreaController.Events
{
    public struct DeckEventOnWallDestroyed : IDeckEvent
    {
        public Vector2Int position { get; private set; }

        public static DeckEventOnWallDestroyed Create(Vector2Int position)
        {
            return new DeckEventOnWallDestroyed()
            {
                position = position
            };
        }
    }
}