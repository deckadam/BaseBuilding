using System;
using Deck.Data.Component;
using Deck.Component;
using Deck.Data.Damage;

namespace Deck.Data.Agent
{
    [Serializable]
    public class DeckAgentData : DeckComponentHolder
    {
        public DeckHealthData healthData;
        public DeckDamageData damageData;
    }
}