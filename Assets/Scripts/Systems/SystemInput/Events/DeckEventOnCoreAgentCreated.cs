using Deck.EventManager;
using Deck.Components.Core;

namespace Deck.InputHandling.Events
{
    public class DeckEventOnCoreAgentCreated : IDeckEvent
    {
        public DeckAgentCore agent { get; private set; }

        public static DeckEventOnCoreAgentCreated Create(DeckAgentCore agent)
        {
            return new() { agent = agent };
        }
    }
}