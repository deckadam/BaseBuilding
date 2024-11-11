using System.Collections.Generic;
using Deck.Data.Currency;
using Deck.Services.Implementations.Currency;
using UnityEngine;

namespace Deck.UI.Currency
{
    public class DeckUICurrency : DeckUIBase
    {
        [SerializeField] private RectTransform container;

        private List<DeckUICurrencyDisplayer> _activeDisplayers = new();
        private DeckCurrency[] _currencies;

        public override void AfterGameSessionInitialized()
        {
            _currencies = Deck.GetService<DeckServiceCurrency>().GetCurrencies();
            for (var index = 0; index < _currencies.Length; index++)
            {
                _currencies[index] = DeckCurrency.Create(_currencies[index]);
                var newDisplayer = InstanceProvider.RentUIElement<DeckUICurrencyDisplayer>();
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