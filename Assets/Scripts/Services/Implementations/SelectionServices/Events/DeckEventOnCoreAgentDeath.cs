using Deck.EventManager;
using Deck.Components.Core;

namespace Deck.Services
{
    public class DeckEventOnCoreAgentDeath : DeckEvent
    {
        public DeckAgentCore agent { get; private set; }

        public static DeckEventOnCoreAgentDeath Create(DeckAgentCore agent)
        {
            return new DeckEventOnCoreAgentDeath
            {
                agent = agent
            };
        }
    }
}