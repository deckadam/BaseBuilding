using Deck.EventManager;
using Deck.InGame.Agent.Core;

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