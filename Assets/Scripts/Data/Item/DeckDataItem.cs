using System;
using System.Linq;
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
        [SerializeField] private string[] tags;

        public string Name => name;
        public Sprite Icon => icon;
        public DeckItemVisual Representation => representation;
        public string AnimationName => animationName;
        public int Amount => amount;
        public bool Holdable => holdable;
        public string[] Tags => tags;
        public bool HasTag(string tag) => tags.Contains(tag);


        public static DeckDataItem Create(string name, Sprite icon, int amount, DeckItemVisual representation, bool holdable, string animationName, string[] tags)
        {
            var newItem = CreateInstance<DeckDataItem>();
            newItem.name = name;
            newItem.icon = icon;
            newItem.amount = amount;
            newItem.representation = representation;
            newItem.holdable = holdable;
            newItem.animationName = animationName;
            newItem.tags = tags;
            return newItem;
        }

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