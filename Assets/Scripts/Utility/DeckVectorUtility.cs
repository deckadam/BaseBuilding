using System.Collections.Generic;
using UnityEngine;

namespace Deck.Utility
{
    public static class DeckVectorUtility
    {
        public static Vector3 ToVector3(this Vector2Int position)
        {
            return new Vector3(position.x, 0, position.y);
        }

        public static Vector2Int ToVector2Int(this Vector3 position)
        {
            int x;
            if (position.x % 1f > 0.55f)
            {
                x = (int)position.x + 1;
            }
            else
            {
                x = (int)position.x;
            }

            int z;
            if (position.z % 1f > 0.55f)
            {
                z = (int)position.z + 1;
            }
            else
            {
                z = (int)position.z;
            }

            return new Vector2Int(x, z);
        }

        public static Vector3Int ToVector3Int(this Vector3 position)
        {
            var x = Mathf.RoundToInt(position.x);
            var y = Mathf.RoundToInt(position.y);
            var z = Mathf.RoundToInt(position.z);
            return new Vector3Int(x, y, z);
        }

        public static Vector3 GetPlaneMiddlePosition(Vector2Int start, Vector2Int end)
        {
            return (start.ToVector3() + end.ToVector3()) / 2f;
        }

        public static Quaternion GetPlaneRotation(Vector2Int start, Vector2Int end)
        {
            var direction = end - start;
            return Quaternion.LookRotation(direction.ToVector3(), Vector3.up);
        }

        public static List<Vector2Int> GetRectFromPoints(this Vector3[] points)
        {
            
            var firstPos = points[0].ToVector2Int();
            var secondPos = points[1].ToVector2Int();

            var minX = Mathf.Min(firstPos.x, secondPos.x);
            var maxX = Mathf.Max(firstPos.x, secondPos.x);

            var minY = Mathf.Min(firstPos.y, secondPos.y);
            var maxY = Mathf.Max(firstPos.y, secondPos.y);

            var rectBuildPositions = new List<Vector2Int>();
            // Bottom edge
            for (int x = minX; x <= maxX; x++)
                rectBuildPositions.Add(new Vector2Int(x, minY));

            // Top edge
            for (int x = minX; x <= maxX; x++)
                rectBuildPositions.Add(new Vector2Int(x, maxY));

            // Left edge
            for (int y = minY + 1; y < maxY; y++)
                rectBuildPositions.Add(new Vector2Int(minX, y));

            // Right edge
            for (int y = minY + 1; y < maxY; y++)
                rectBuildPositions.Add(new Vector2Int(maxX, y));

            return rectBuildPositions;
        }
    }
}