using Deck.Components.Core;
using Deck.EventManager;

namespace Deck.Services.Selection.Events
{
    public class DeckEventOnCoreAgentDeath : IDeckEvent
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