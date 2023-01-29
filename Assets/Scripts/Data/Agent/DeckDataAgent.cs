using System;
using Data.Component;
using Deck.Data.Component;
using Deck.Data.Damage;
using UnityEngine;

namespace Deck.Data.Agent
{
    [Serializable]
    public class DeckDataAgent
    {
        [SerializeField] private DeckDataHealth dataHealth;
        [SerializeField] private DeckDataDamage dataDamage;
        [SerializeField] private DeckDataMovement dataMovement;

        public DeckComponentData[] GetDataArray()
        {
            return new DeckComponentData[] {dataHealth, dataDamage, dataMovement};
        }
    }
}