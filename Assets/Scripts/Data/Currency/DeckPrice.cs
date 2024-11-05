using System;
using UnityEngine;

namespace Deck.Data.Currency
{
    [Serializable]
    public class DeckPrice
    {
        [SerializeField] private DeckCurrencyType currencyType;
        [SerializeField] private int amount;

        public DeckCurrencyType CurrencyType => currencyType;
        public int Amount => amount;
    }
}