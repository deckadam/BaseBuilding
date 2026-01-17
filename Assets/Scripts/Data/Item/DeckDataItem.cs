using System;
using System.Collections.Generic;
using Base;
using UnityEngine;

namespace Data.Item
{
    [CreateAssetMenu(fileName = "Deck Data Item", menuName = "Deck/Data/Item", order = 0)]
    public class DeckDataItem : ScriptableObject
    {
        [SerializeField] private new string name;
        [SerializeField] private Sprite icon;
        [SerializeField] private int amount;
        [SerializeField] private DeckItemVisual representation;
        [SerializeField] private string animationName;
        [SerializeField] private List<DeckActionTag> tags;

        public string Name => name;
        public Sprite Icon => icon;
        public DeckItemVisual Representation => representation;
        public string AnimationName => animationName;
        public int Amount => amount;
        public List<DeckActionTag> Tags => tags;
        public bool HasTag(DeckActionTag tag) => tags.Contains(tag);

        public bool HasTag(DeckActionTag[] tag)
        {
            foreach (var t in tag)
            {
                if (tags.Contains(t))
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasTag(List<DeckActionTag> tag)
        {
            foreach (var t in tag)
            {
                if (tags.Contains(t))
                {
                    return true;
                }
            }

            return false;
        }

        public static DeckDataItem Create(string name, Sprite icon, int amount, DeckItemVisual representation, string animationName, List<DeckActionTag> tags)
        {
            var newItem = CreateInstance<DeckDataItem>();
            newItem.name = name;
            newItem.icon = icon;
            newItem.amount = amount;
            newItem.representation = representation;
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

        public void SetToAmount(int amountToSet)
        {
            if (amountToSet < 0)
            {
                throw new Exception("Please only enter positive values");
            }

            amount = amountToSet;
        }

        public void ResetAmount()
        {
            amount = 0;
        }
    }
}