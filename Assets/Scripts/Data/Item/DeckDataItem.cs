using System;
using Deck.Item;
using UnityEngine;

namespace Deck.Data.Item
{
    [CreateAssetMenu(fileName = "Deck Data Item", menuName = "Deck/Data/Item", order = 0)]
    public class DeckDataItem : ScriptableObject
    {
        [SerializeField] private new string name;
        [SerializeField] private Sprite icon;
        [SerializeField] private int amount;
        [SerializeField] private DeckItemVisual representation;
        [SerializeField] private bool holdable;
        [SerializeField] private string animationName;

        public string Name => name;
        public Sprite Icon => icon;
        public DeckItemVisual Representation => representation;
        public string AnimationName => animationName;
        public int Amount => amount;
        public bool Holdable => holdable;

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