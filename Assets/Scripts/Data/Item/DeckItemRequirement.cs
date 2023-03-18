using System;

namespace Deck.Data.Item
{
    [Serializable]
    public class DeckItemRequirement
    {
        public DeckDataItem item;
        public int requiredAmount;
    }
}