namespace Deck.InGame.Agent.Building.Stats
{
    public struct DeckStat
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }

        public DeckStat(string name, string value, string description)
        {
            Name = name;
            Value = value;
            Description = description;
        }
    }
}