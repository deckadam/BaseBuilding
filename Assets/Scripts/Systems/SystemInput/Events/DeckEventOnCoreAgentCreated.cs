using Deck.Components.Core;
using Deck.EventManager;

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