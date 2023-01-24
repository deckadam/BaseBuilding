using System;
using System.Collections.Generic;
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

        private Dictionary<Type, object> _data;

        public void Initialize()
        {
            _data = new Dictionary<Type, object>();
            _data[typeof(DeckDataHealth)] = dataHealth;
            _data[typeof(DeckDataDamage)] = dataDamage;
            _data[typeof(DeckDataMovement)] = dataMovement;
        }

        public T GetData<T>()
        {
            return (T) _data[typeof(T)];
        }
    }
}