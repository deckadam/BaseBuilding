using System;
using UnityEngine;

namespace Data.Item
{
    [Serializable]
    public class DeckItemRequirement
    {
        [SerializeField] private DeckDataItem item;
        [SerializeField] private int requiredAmount;

        public DeckDataItem Item => item;
        public int RequiredAmount => requiredAmount;
    }
}