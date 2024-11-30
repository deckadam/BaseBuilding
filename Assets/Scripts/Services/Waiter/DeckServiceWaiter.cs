using System.Collections.Generic;
using Deck.InGame.Agent.Waiter;
using UnityEngine;

namespace Deck.Waiter
{
    public class DeckServiceWaiter : DeckServiceBase
    {
        private List<DeckAgentWaiter> _availableWaiters = new();

        public override void BeforeGameSessionInitialized()
        {
            _availableWaiters.Clear();
        }

        public void RegisterAvailableWaiter(DeckAgentWaiter waiter)
        {
            _availableWaiters.Add(waiter);
            Debug.LogError(_availableWaiters.Count);
        }

        public void RemoveAvailableWaiter(DeckAgentWaiter waiter)
        {
            _availableWaiters.Remove(waiter);
        }

        public bool TryGetAvailableWaiter(out DeckAgentWaiter waiter)
        {
            if (_availableWaiters.Count > 0)
            {
                waiter = _availableWaiters[0];
                _availableWaiters.RemoveAt(0);
                return true;
            }

            waiter = null;
            return false;
        }
    }
}