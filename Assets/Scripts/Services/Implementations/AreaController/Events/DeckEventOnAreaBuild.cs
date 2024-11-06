using System.Collections.Generic;
using Deck.EventManager;
using UnityEngine;

namespace Services.Implementations.AreaController.Events
{
    public struct DeckEventOnAreaBuild : IDeckEvent
    {
        public List<Vector2Int> cellPositions { get; private set; }
        public Vector3[] positions { get; private set; }

        public static DeckEventOnAreaBuild Create(Vector3[] positions, List<Vector2Int> cellPositions)
        {
            return new DeckEventOnAreaBuild()
            {
                positions = positions,
                cellPositions = cellPositions
            };
        }
    }
}