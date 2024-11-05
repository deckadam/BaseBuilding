using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Deck.Components.Building
{
    public class DeckNotification : DeckUIElement
    {
        [SerializeField] private TextMeshProUGUI display;
        [SerializeField] private CanvasGroup group;

        public void SetMessage(string message)
        {
            display.text = message;
            rectTransform.DOAnchorPosY(Screen.width / 5f, 3.5f).SetDelay(1.5f);
            group.DOFade(0f, 1f).SetDelay(3f);
            Destroy(gameObject, 6f);
        }
    }
}