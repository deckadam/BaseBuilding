using Deck.EventManager;
using Deck.Agent;

namespace Deck.Services.Implementations.MapService.Events
{
    public class DeckOnPlayerSpawned:DeckEvent
    {
        public DeckCoreAgent agent { get; private set; }
        public static DeckOnPlayerSpawned Create(DeckCoreAgent agent)
        {
            return new()
            {
                agent = agent
            };
        }
    }
}