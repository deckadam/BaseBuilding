using Deck.EventManager;
using Deck.Agent;

namespace Deck.Services.Implementations.MapService.Events
{
    public class DeckOnPlayerSpawned:DeckEvent
    {
        public DeckAgentCore agent { get; private set; }
        public static DeckOnPlayerSpawned Create(DeckAgentCore agent)
        {
            return new()
            {
                agent = agent
            };
        }
    }
}