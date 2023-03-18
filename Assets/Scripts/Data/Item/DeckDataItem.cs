using System;
using UnityEngine;

namespace Deck.Data.Item
{
    [CreateAssetMenu(fileName = "Deck Data Item", menuName = "Deck/Data/Item", order = 0)]
    public class DeckDataItem : ScriptableObject
    {
        [SerializeField] private new string name;
        [SerializeField] private Sprite icon;
        [SerializeField] private int amount;

        public string GetName() => name;
        public Sprite GetIcon() => icon;
        public int GetAmount() => amount;

        public void AddToAmount(int amountToAdd)
        {
            if (amountToAdd < 0)
            {
                throw new Exception("Please only enter positive values");
            }

            amount += amountToAdd;
        }

        public void RemoveFromAmount(int amountToRemove)
        {
            if (amountToRemove < 0)
            {
                throw new Exception("Please only enter positive values");
            }

            amount -= amountToRemove;
        }

        public void ResetAmount()
        {
            amount = 0;
        }
    }
}