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
            var x = Mathf.RoundToInt(position.x);
            var z = Mathf.RoundToInt(position.z);
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
    }
}