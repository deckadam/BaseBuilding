using Deck.Commands;

namespace Deck.UI.Stats
{
    public struct DeckStatGroup
    {
        public DeckComponent Component;
        public DeckStat[] Stats;
        
        public DeckStatGroup(DeckStat[] stats,DeckComponent component)
        {
            Stats = stats;
            Component = component;
        }
    }
}