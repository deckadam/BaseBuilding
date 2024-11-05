using Deck.Data.Currency;
using Deck.Save;
using Deck.Services;
using UnityEngine;
using UnityEngine.Serialization;

namespace Services.Implementations.Currency
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

        public void ChangeValueRelative(DeckCurrencyType currencyType, int amountToAdd)
        {
            var currency = _currentCurrencies[(int)currencyType];
            currency.ChangeValueRelative(amountToAdd);
        }

        public void ChangeValue(DeckCurrencyType currencyType, int newAmount)
        {
            var currency = _currentCurrencies[(int)currencyType];
            currency.ChangeValue(newAmount);
        }

        public bool HasEnoughAmount(DeckCurrencyType currencyType, int amountToCheck)
        {
            return _currentCurrencies[(int)currencyType].HasEnoughAmount(amountToCheck);
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
                Debug.LogError(currency.SaveKey +"  " + currency.Amount);
            }
        }
    }
}