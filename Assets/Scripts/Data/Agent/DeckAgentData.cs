using System;
using System.Collections.Generic;
using Deck.Data.Component;
using Deck.Data.Damage;

namespace Deck.Data.Agent
{
    [Serializable]
    public class DeckAgentData
    {
        public DeckHealthData healthData;
        public DeckDamageData damageData;

        private Dictionary<Type, object> _data;

        public void Initialize()
        {
            _data = new Dictionary<Type, object>();
            _data[typeof(DeckHealthData)] = healthData;
            _data[typeof(DeckDamageData)] = damageData;
        }

        public T GetData<T>()
        {
            return (T) _data[typeof(T)];
        }
    }
}