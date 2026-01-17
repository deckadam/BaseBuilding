using UnityEngine;

namespace Utility.Iterators
{
    public static class DeckNeighbourIterator
    {
        public static Vector2Int[] GetNeighbours(this Vector2Int position)
        {
            return new[]
            {
                new Vector2Int(position.x + 1, position.y),
                new Vector2Int(position.x - 1, position.y),
                new Vector2Int(position.x, position.y + 1),
                new Vector2Int(position.x, position.y - 1)
            };
        }

        public static Vector2Int[] GetNeighboursWithSelf(this Vector2Int position)
        {
            return new[]
            {
                new Vector2Int(position.x, position.y),
                new Vector2Int(position.x + 1, position.y),
                new Vector2Int(position.x - 1, position.y),
                new Vector2Int(position.x, position.y + 1),
                new Vector2Int(position.x, position.y - 1)
            };
        }

        public static Vector2Int[] GetDiagonalNeighbours(this Vector2Int position)
        {
            return new[]
            {
                new Vector2Int(position.x + 1, position.y + 1),
                new Vector2Int(position.x - 1, position.y + 1),
                new Vector2Int(position.x + 1, position.y - 1),
                new Vector2Int(position.x - 1, position.y - 1)
            };
        }


        public static Vector2Int[] GetRectNeighbours(this Vector2Int position)
        {
            return new[]
            {
                new Vector2Int(position.x + 1, position.y),
                new Vector2Int(position.x - 1, position.y),
                new Vector2Int(position.x, position.y + 1),
                new Vector2Int(position.x, position.y - 1),
                new Vector2Int(position.x + 1, position.y + 1),
                new Vector2Int(position.x - 1, position.y + 1),
                new Vector2Int(position.x + 1, position.y - 1),
                new Vector2Int(position.x - 1, position.y - 1)
            };
        }

        public static Vector2Int[] GetDiagonalNeighboursWithSelf(this Vector2Int position)
        {
            return new[]
            {
                new Vector2Int(position.x, position.y),
                new Vector2Int(position.x + 1, position.y + 1),
                new Vector2Int(position.x - 1, position.y + 1),
                new Vector2Int(position.x + 1, position.y - 1),
                new Vector2Int(position.x - 1, position.y - 1)
            };
        }

        public static Vector2Int[] GetNeighbours(this Vector2Int position, int multiplier)
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