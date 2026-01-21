using EventManager;
using InGame.Agent.Waiter;

namespace Services.Selection.Events
{
    public struct DeckEventOnCoreAgentDeath : IDeckEvent
    {
        public DeckAgentWaiter agent { get; private set; }

        public static DeckEventOnCoreAgentDeath Create(DeckAgentWaiter agent)
        {
            return new DeckEventOnCoreAgentDeath
            {
                agent = agent
            };
        }
    }
}