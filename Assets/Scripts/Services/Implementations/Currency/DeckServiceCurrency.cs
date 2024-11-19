using System.Linq;
using Deck.Data.Currency;
using Deck.Save;
using UnityEngine;

namespace Deck.Services.Implementations.Currency
{
    public class DeckServiceCurrency : DeckServiceBase
    {
        [SerializeField] private DeckCurrency[] _defaultCurrencies;

        private DeckCurrency[] _currentCurrencies;

        public override void BeforeGameSessionInitialized()
        {
            _currentCurrencies = new DeckCurrency[_defaultCurrencies.Length];

            for (var i = 0; i < _currentCurrencies.Length; i++)
            {
                var saveKey = _defaultCurrencies[i].SaveKey;
                if (DeckSaveSystem.HasData(saveKey))
                {
                    var amount = DeckSaveSystem.GetData<int>(saveKey);
                    _currentCurrencies[i] = DeckCurrency.Create(_defaultCurrencies[i], amount);
                }
                else
                {
                    _currentCurrencies[i] = DeckCurrency.Create(_defaultCurrencies[i]);
                }
            }
        }

        public void ChangeValueRelative(DeckPrice[] prices, int multiplier, bool add)
        {
            var mul = add ? 1 : -1;
            foreach (var price in prices)
            {
                var currency = _currentCurrencies.First(item => item.CurrencyType == price.CurrencyType);
                currency.ChangeValueRelative(mul * price.Amount * multiplier);
            }
        }

        public void ChangeValueRelative(DeckPrice[] prices, bool add)
        {
            var mul = add ? 1 : -1;
            foreach (var price in prices)
            {
                var currency = _currentCurrencies.First(item => item.CurrencyType == price.CurrencyType);
                currency.ChangeValueRelative(price.Amount * mul);
            }
        }

        public void ChangeValueRelative(DeckCurrencyType currencyType, int amount)
        {
            var currency = _currentCurrencies.First(item => item.CurrencyType == currencyType);
            currency.ChangeValueRelative(amount);
        }

        public void ChangeValueRelative(DeckPrice price)
        {
            var currency = _currentCurrencies.First(item => item.CurrencyType == price.CurrencyType);
            currency.ChangeValueRelative(price.Amount);
        }

        public void ChangeValue(DeckCurrencyType currencyType, int newAmount)
        {
            var currency = _currentCurrencies[(int)currencyType];
            currency.ChangeValue(newAmount);
        }

        public bool CanAfford(DeckPrice[] prices, int multiplier)
        {
            foreach (var deckPrice in prices)
            {
                if (!CanAfford(deckPrice, multiplier))
                {
                    return false;
                }
            }

            return true;
        }

        public bool CanAfford(DeckPrice[] prices)
        {
            foreach (var deckPrice in prices)
            {
                if (!CanAfford(deckPrice))
                {
                    return false;
                }
            }

            return true;
        }

        public bool CanAfford(DeckPrice price, int multiplier)
        {
            return _currentCurrencies[(int)price.CurrencyType].HasEnoughAmount(price.Amount * multiplier);
        }

        public bool CanAfford(DeckPrice price)
        {
            return _currentCurrencies[(int)price.CurrencyType].HasEnoughAmount(price.Amount);
        }

        public DeckCurrency[] GetCurrencies()
        {
            return _currentCurrencies;
        }

        public override void BeforeSaveRequest()
        {
            foreach (var currency in _currentCurrencies)
            {
                DeckSaveSystem.SetData(currency.SaveKey, currency.Amount);
            }
        }
    }
}