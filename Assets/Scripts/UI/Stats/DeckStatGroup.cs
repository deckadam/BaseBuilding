using Deck.Commands;

namespace Deck.InGame.Agent.Building.Stats
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