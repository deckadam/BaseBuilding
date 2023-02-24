using System;
using Data.Component;
using Deck.Data.Component;
using UnityEngine;

namespace Deck.Data.Agent
{
    [Serializable]
    public class DeckDataAgentChest
    {
        [SerializeField] private DeckDataHealth dataHealth;

        public DeckComponentData[] GetDataArray()
        {
            return new DeckComponentData[] { dataHealth };
        }
    }
}