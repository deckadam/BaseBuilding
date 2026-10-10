using System.Collections.Generic;

namespace Utility
{
    public static class DeckDataStructureUtility
    {
        public static bool IsNullOrEmpty<T>(this T[] array)
        {
            if (array == null)
            {
                return true;
            }

            if (array.Length == 0)
            {
                return true;
            }

            return false;
        }
        
        public static bool IsNullOrEmpty<T>(this IReadOnlyList<T> array)
        {
            if (array == null)
            {
                return true;
            }

            if (array.Count == 0)
            {
                return true;
            }

            return false;
        }
    }
}