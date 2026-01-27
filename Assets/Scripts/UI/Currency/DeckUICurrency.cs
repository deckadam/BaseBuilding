using System.Collections.Generic;
using Data.Currency;
using Services;
using Services.Currency;
using UnityEngine;

namespace UI.Currency
{
    public class DeckUICurrency : DeckUIBase
    {
        [SerializeField] private RectTransform container;

        private List<DeckUICurrencyDisplay> _activeDisplays = new();
        private DeckCurrency[] _currencies;

        public override void AfterGameSessionInitialized()
        {
            _currencies = DeckServiceProvider.GetService<DeckServiceCurrency>().GetCurrencies();
            for (var index = 0; index < _currencies.Length; index++)
            {
                _currencies[index] = DeckCurrency.Create(_currencies[index]);
                var newDisplay = InstanceProvider.RentUIElement<DeckUICurrencyDisplay>();
                newDisplay.transform.SetParent(container);
                newDisplay.SetCurrency(_currencies[index]);
                _activeDisplays.Add(newDisplay);
            }
        }

        public override void BeforeGameSceneUnloaded()
        {
            foreach (var currencyDisplay in _activeDisplays)
            {
                InstanceProvider.ReturnUIElement(currencyDisplay);
            }
        }
    }
}