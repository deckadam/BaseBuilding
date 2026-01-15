using EventManager;
using InGame.Agent.Waiter;

namespace Systems.SystemInput.Events
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