using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utility.DataStructure
{
    public static class DeckClassUtility
    {
        public static T GetRandomElement<T>(this HashSet<T> hashSet)
        {
            return hashSet.ElementAt(Random.Range(0, hashSet.Count));
        }
    }
}