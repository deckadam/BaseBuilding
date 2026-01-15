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

        private List<DeckUICurrencyDisplay> _activeDisplayers = new();
        private DeckCurrency[] _currencies;

        public override void AfterGameSessionInitialized()
        {
            _currencies = DeckServiceProvider.GetService<DeckServiceCurrency>().GetCurrencies();
            for (var index = 0; index < _currencies.Length; index++)
            {
                _currencies[index] = DeckCurrency.Create(_currencies[index]);
                var newDisplayer = InstanceProvider.RentUIElement<DeckUICurrencyDisplay>();
                newDisplayer.transform.SetParent(container);
                newDisplayer.SetCurrency(_currencies[index]);
                _activeDisplayers.Add(newDisplayer);
            }
        }

        public override void BeforeGameSceneUnloaded()
        {
            foreach (var currencyDisplayer in _activeDisplayers)
            {
                InstanceProvider.ReturnUIElement(currencyDisplayer);
            }
        }
    }
}