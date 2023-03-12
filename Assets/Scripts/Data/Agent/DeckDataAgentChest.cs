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
        [SerializeField] private DeckDataMovement dataMovement;

        public DeckComponentData[] GetDataArray()
        {
            return new DeckComponentData[] { dataHealth, dataMovement };
        }
    }
}