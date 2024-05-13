using Unity.VisualScripting;
using UnityEngine;

namespace Deck.Utility.MonoBehaviours
{
    public static class DeckMonoExtensions
    {
        public static bool TryGetComponentInParent<T>(this Object transform, out T component) where T : Component
        {
            component = transform.GetComponentInParent<T>();
            return component != null;
        }
    }
}