using UnityEngine;

namespace Deck.UI
{
    public class DeckPopUpBase : MonoBehaviour
    {
        [SerializeField] protected RectTransform rect;

        private Vector2 _clickPosition;
        private Vector2 _startPosition;

        private void OnValidate()
        {
            rect = GetComponent<RectTransform>();
        }

        public void OnPointerClick()
        {
            Debug.LogError("Clicked");
            _clickPosition = Input.mousePosition;
            _startPosition = rect.anchoredPosition;
        }

        public void OnPointerDrag()
        {
            var delta = (Vector2) Input.mousePosition - _clickPosition;
            rect.anchoredPosition = delta + _startPosition;
        }
    }
}