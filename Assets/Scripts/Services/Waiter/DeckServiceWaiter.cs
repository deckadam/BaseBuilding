using System.Collections.Generic;
using Deck.InGame.Agent.Waiter;

namespace Deck.Waiter
{
    public class DeckServiceWaiter : DeckServiceBase
    {
        private Queue<DeckAgentWaiter> _availableWaiters = new();

        public override void BeforeGameSessionInitialized()
        {
            _availableWaiters.Clear();
        }

        public void RegisterAvailableWaiter(DeckAgentWaiter waiter)
        {
            _availableWaiters.Enqueue(waiter);
        }

        public bool TryGetAvailableWaiter(out DeckAgentWaiter waiter)
        {
            return _availableWaiters.TryDequeue(out waiter);
        }
    }
}