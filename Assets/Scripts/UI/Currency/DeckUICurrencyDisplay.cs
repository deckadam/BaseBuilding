using Data.Currency;
using Deck.Base;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Currency
{
    public class DeckUICurrencyDisplay : DeckUIElement
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI amountText;
        [SerializeField] private Image displayImage;

        private DeckCurrency _currency;

        public void SetCurrency(DeckCurrency currency)
        {
            _currency = currency;
            nameText.text = currency.DisplayName;
            amountText.text = currency.Amount.ToString();
            displayImage.sprite = currency.DisplaySprite;

            _currency.OnAmountChanged += OnValueChanged;
        }

        private void OnValueChanged()
        {
            amountText.text = _currency.Amount.ToString();
        }
    }
}