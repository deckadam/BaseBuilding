using UnityEngine;

namespace Deck.Utility.Iterators
{
    public static class DeckIterator
    {
        public static Vector2Int[] GetNeighbourIterator(Vector2Int position)
        {
            return new[]
            {
                new Vector2Int(position.x + 1, position.y),
                new Vector2Int(position.x - 1, position.y),
                new Vector2Int(position.x, position.y + 1),
                new Vector2Int(position.x, position.y - 1)
            };
        }

        public static Vector2Int[] GetNeighbourIterator(Vector2Int position, int multiplier)
        {
            return new[]
            {
                new Vector2Int(position.x + multiplier, position.y),
                new Vector2Int(position.x - multiplier, position.y),
                new Vector2Int(position.x, position.y + multiplier),
                new Vector2Int(position.x, position.y - multiplier)
            };
        }
    }
}