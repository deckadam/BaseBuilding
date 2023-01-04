using System.Collections.Generic;
using Deck.Map;
using Deck.Player;
using Deck.Utility.Logger;

namespace Deck.Services.Implementations.Level
{
    public class DeckLevelService : DeckServiceBase
    {
        public List<DeckCoreAgent> agents { get; private set; }
        public DeckCoreGrid deckCoreGrid { get; private set; }
        public DeckGamePlayMap map { get; private set; }


        public override void Initialize()
        {
            agents = new List<DeckCoreAgent>();
        }

        public void AddCoreAgent(DeckCoreAgent agent)
        {
            DeckLogger.Level("Setting player");
            agents.Add(agent);
        }

        public void SetGrid(DeckCoreGrid deckCoreGrid)
        {
            DeckLogger.Level("Setting grid");
            this.deckCoreGrid = deckCoreGrid;
        }

        public void SetMap(DeckGamePlayMap map)
        {
            this.map = map;
        }
    }
}