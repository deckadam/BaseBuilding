using System.Linq;
using UnityEngine;

namespace Utility.MonoBehaviours
{
    public class DeckTransformBinder : MonoBehaviour
    {
        [SerializeField] private string[] keys;

        public bool ContainsKey(string key, out string result)
        {
            result = key;
            return keys.Contains(key);
        }

        public string[] GetKeys() => keys;
    }
}