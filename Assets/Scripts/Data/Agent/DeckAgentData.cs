using System;
using Deck.Test.Data.Component;

namespace Deck.Test.Data.Agent
{
    [Serializable]
    public class DeckAgentData : DeckComponentHolder
    {
        public DeckHealthComponentData healthComponentData;
    }
}