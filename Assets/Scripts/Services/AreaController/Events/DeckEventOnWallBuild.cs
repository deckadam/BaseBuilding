using Deck.EventManager;
using UnityEngine;

namespace Deck.Services.AreaController.Events
{
    public struct DeckEventOnWallBuild : IDeckEvent
    {
        public Vector2Int position { get; private set; }

        public static DeckEventOnWallBuild Create(Vector2Int positions)
        {
            return new DeckEventOnWallBuild()
            {
                position = positions
            };
        }
    }
}