using UnityEngine;

namespace Utility.Array
{
    public static class DeckArrayUtility
    {
        public static Vector2Int[] CreateCopy(this Vector2Int[] array)
        {
            var result = new Vector2Int[array.Length];
            for (var i = 0; i < array.Length; i++)
            {
                result[i] = array[i];
            }

            return result;
        }
    }
}