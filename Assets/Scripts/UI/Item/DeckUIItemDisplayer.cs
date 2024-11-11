using System;
using Deck.Data.Item;
using TMPro;
using UnityEngine;

namespace Deck.UI.Item
{
    public class DeckUIItemDisplayer : DeckUIWorldDisplay
    {
        [SerializeField] private RectTransform backGround;
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private Vector2 padding;
        private Action _onClick;

        public void SetData(DeckDataItem item, Action onClick = null)
        {
            _onClick = onClick;
            if (item.Amount == 1)
            {
                label.text = item.Name;
            }
            else
            {
                label.text = item.Name + " X" + item.Amount;
            }


            var width = label.preferredWidth < label.rectTransform.sizeDelta.x ? label.preferredWidth : label.rectTransform.sizeDelta.x;
            var size = new Vector2(width + padding.x, label.preferredHeight + padding.y);
            backGround.sizeDelta = size;
        }

        public void OnClicked()
        {
            _onClick?.Invoke();
        }
    }
}