using Deck.EventManager;
using Deck.InGame.Agent.Waiter;

namespace Deck.InputHandling.Events
{
    public class DeckEventOnCoreAgentCreated : IDeckEvent
    {
        public DeckAgentWaiter agent { get; private set; }

        public static DeckEventOnCoreAgentCreated Create(DeckAgentWaiter agent)
        {
            return new() { agent = agent };
        }
    }
}