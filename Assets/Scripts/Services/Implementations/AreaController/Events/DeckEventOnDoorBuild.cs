using Deck.EventManager;
using UnityEngine;

namespace Services.Implementations.AreaController.Events
{
    public struct DeckEventOnDoorBuild : IDeckEvent
    {
        public Vector2Int position { get; private set; }

        public static DeckEventOnDoorBuild Create(Vector2Int position)
        {
            return new DeckEventOnDoorBuild()
            {
                position = position
            };
        }
    }
}