using System;
using Data.Component;
using Deck.Data.Component;
using Deck.Data.Damage;
using UnityEngine;

namespace Deck.Data
{
    [Serializable]
    public class DeckDataAgentCore
    {
        [SerializeField] private DeckDataHealth dataHealth;
        [SerializeField] private DeckDataDamage dataDamage;
        [SerializeField] private DeckDataMovement dataMovement;

        public DeckDataComponent[] GetDataArray()
        {
            return new DeckDataComponent[] { dataHealth, dataDamage, dataMovement };
        }
    }
}