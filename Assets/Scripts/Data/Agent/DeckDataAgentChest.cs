using System;
using Data.Component;
using Deck.Data.Component;
using UnityEngine;

namespace Deck.Data
{
    [Serializable]
    public class DeckDataAgentChest
    {
        [SerializeField] private DeckDataHealth dataHealth;
        [SerializeField] private DeckDataMovement dataMovement;

        public DeckDataComponent[] GetDataArray()
        {
            return new DeckDataComponent[] { dataHealth, dataMovement };
        }
    }
}