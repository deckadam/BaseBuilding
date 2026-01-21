using EventManager;
using InGame.Agent.Waiter;

namespace Systems.SystemInput.Events
{
    public struct DeckEventOnCoreAgentCreated : IDeckEvent
    {
        public DeckAgentWaiter agent { get; private set; }

        public static DeckEventOnCoreAgentCreated Create(DeckAgentWaiter agent)
        {
            return new DeckEventOnCoreAgentCreated { agent = agent };
        }
    }
}