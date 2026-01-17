using UnityEngine;

namespace Utility.MonoBehaviours
{
    public static class DeckMonoExtensions
    {
        public static bool TryGetComponentInParent<T>(this GameObject transform, out T component) where T : Component
        {
            component = transform.GetComponentInParent<T>();
            return component != null;
        }
        
        
        public static bool TryGetComponentInParent<T>(this Transform transform, out T component) where T : Component
        {
            component = transform.GetComponentInParent<T>();
            return component != null;
        }
    }
}