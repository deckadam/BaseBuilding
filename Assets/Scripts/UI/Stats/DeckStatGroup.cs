using Deck.Components;

namespace Deck.UI.Stats
{
    public struct DeckStatGroup
    {
        public bool IsValid;
        public DeckComponent Component;
        public DeckStat[] Stats;
        
        public DeckStatGroup(DeckStat[] stats,DeckComponent component)
        {
            Stats = stats;
            Component = component;
            IsValid = true;
        }
    }
}