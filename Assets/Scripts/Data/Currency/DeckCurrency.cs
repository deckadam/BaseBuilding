using System;
using UnityEngine;

namespace Deck.Data.Currency
{
    [CreateAssetMenu(fileName = "Deck Currency", menuName = "Deck/Data/Currency")]
    public class DeckCurrency : ScriptableObject
    {
        [SerializeField] private DeckCurrencyType currencyType;
        [SerializeField] private Sprite displaySprite;
        [SerializeField] private string displayName;
        [SerializeField] private string saveKey;
        [SerializeField] private int amount;

        public Action OnAmountChanged;

        public static DeckCurrency Create(DeckCurrency currency,int amount)
        {
            var instance = CreateInstance<DeckCurrency>();
            instance.displaySprite = currency.displaySprite;
            instance.currencyType = currency.currencyType;
            instance.displayName = currency.displayName;
            instance.saveKey = currency.saveKey;
            instance.amount = amount;
            return instance;
        }
        
        public static DeckCurrency Create(DeckCurrency currency)
        {
            var instance = CreateInstance<DeckCurrency>();
            instance.displaySprite = currency.displaySprite;
            instance.currencyType = currency.currencyType;
            instance.displayName = currency.displayName;
            instance.saveKey = currency.saveKey;
            instance.amount = currency.amount;
            return instance;
        }

        public void ChangeValueRelative(int amountToAdd)
        {
            amount += amountToAdd;
            OnAmountChanged?.Invoke();
        }

        public void ChangeValue(int newAmount)
        {
            amount = newAmount;
            OnAmountChanged?.Invoke();
        }

        public bool HasEnoughAmount(int amountToCheck)
        {
            if (amount >= amountToCheck)
            {
                return true;
            }

            return false;
        }

        public DeckCurrencyType CurrencyType => currencyType;
        public Sprite DisplaySprite => displaySprite;
        public string DisplayName => displayName;
        public string SaveKey => saveKey;
        public int Amount => amount;
    }
}