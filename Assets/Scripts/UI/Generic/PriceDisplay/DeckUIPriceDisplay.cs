using Base;
using Data.Currency;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Generic.PriceText
{
    public class DeckUIPriceDisplay : DeckUIElement
    {
        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private Image priceImage;

        public void Initialize(DeckPrice price, RectTransform parent)
        {
            rectTransform.SetParent(parent);
            priceText.text = price.Amount.ToString();
        }
    }
}