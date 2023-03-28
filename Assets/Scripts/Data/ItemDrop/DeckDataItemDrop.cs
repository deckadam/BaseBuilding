using System;
using System.Collections.Generic;
using System.Linq;
using Data.Component;
using Deck.Data.Item;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Deck.Data.ItemDrop
{
    [CreateAssetMenu(fileName = "Deck Data Item Drop", menuName = "Deck/Data/Item Drop", order = 0)]
    public class DeckDataItemDrop : DeckDataComponent
    {
        [SerializeField] private ItemDrop[] dropables;

        private void OnValidate()
        {
            foreach (var itemDropChance in dropables)
            {
                itemDropChance.AdjustChances();
            }
        }

        public List<(ItemDrop, int)> RollItem()
        {
            var result = new List<(ItemDrop, int)>();
            foreach (var item in dropables)
            {
                var totalWeight = item.GetSet().Sum(set => set.GetChance());
                var randomValue = Random.Range(0f, 1f) * totalWeight;
                var weightSum = 0f;

                var set = item.GetSet();
                for (var index = 0; index < set.Length; index++)
                {
                    var chance = set[index];
                    weightSum += chance.GetChance();

                    if (randomValue <= weightSum)
                    {
                        result.Add((item, chance.GetAmount()));
                        break;
                    }
                }
            }

            return result;
        }

        [Serializable]
        public class ItemDrop
        {
            [SerializeField, OnValueChanged(nameof(AdjustChances))] private ChanceSet[] chances;
            [SerializeField] private DeckDataItem itemToDrop;

            public void AdjustChances()
            {
                var sum = chances.Select(item => item.GetChance()).Sum();

                if (sum == 0)
                {
                    return;
                }

                var ratio = 1f / sum;
                for (var i = 0; i < chances.Length; i++)
                {
                    chances[i].SetChance(chances[i].GetChance() * ratio);
                }
            }

            public ChanceSet[] GetSet() => chances;
            public DeckDataItem GetItem() => itemToDrop;
        }

        [Serializable]
        public class ChanceSet
        {
            [SerializeField, Range(0f, 1f)] private float chance;
            [SerializeField] private int amount;

            public float GetChance() => chance;
            public void SetChance(float newChance) => chance = newChance;

            public int GetAmount() => amount;
            public void SetAmount(int newAmount) => amount = newAmount;
        }
    }
}