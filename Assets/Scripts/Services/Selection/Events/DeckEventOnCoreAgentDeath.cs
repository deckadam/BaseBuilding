using EventManager;
using InGame.Agent.Waiter;

namespace Deck.Services.Selection.Events
{
    public class DeckEventOnCoreAgentDeath : IDeckEvent
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