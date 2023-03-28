using UnityEngine;

namespace Utility.MonoBehaviours
{
    public class DeckTransformBinder : MonoBehaviour
    {
        [SerializeField] private string key;

        public bool IsMatching(string key)
        {
            return key == this.key;
        }

        public string GetKey() => key;
    }
}