using UnityEngine;

namespace Deck.Utility.Constants.Hotkey.Events
{
    public class DeckHotKeyPiece : MonoBehaviour
    {
        public void Highlight()
        {
            transform.localScale = Vector3.one * 1.2f;
        }

        public void Normalize()
        {
            transform.localScale = Vector3.one;
        }
    }
}