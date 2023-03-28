using System;
using Data.Component;
using Data.Component.Tree;
using Deck.Data.Component;
using Deck.Data.ItemDrop;
using UnityEngine;
using UnityEngine.Serialization;

namespace Deck.Data
{
    [Serializable]
    public class DeckDataAgentHarvestable
    {
        [SerializeField] private DeckDataItemDrop itemDrop;
        [SerializeField] private DeckDataHealth healthData;
        [SerializeField] private DeckDataAnimationTreeShake shakeDataAnimation;
        
        public DeckDataComponent[] GetDataArray()
        {
            return new DeckDataComponent[] { itemDrop, healthData, shakeDataAnimation };
        }
    }
}