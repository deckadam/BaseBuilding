using System;
using UnityEngine;

namespace Deck.Data.Currency
{
    [Serializable]
    public struct DeckPrice
    {
        [SerializeField] private DeckCurrencyType currencyType;
        [SerializeField] private int amount;

        public DeckPrice(int amount, DeckCurrencyType currencyType)
        {
            this.amount = amount;
            this.currencyType = currencyType;
        }
        
        public DeckCurrencyType CurrencyType => currencyType;
        public int Amount => amount;
    }
}