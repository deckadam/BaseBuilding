using Deck.EventManager;
using Deck.InputHandling.Events;
using Deck.Player;
using UnityEngine.AI;

namespace Deck.Components
{
    public class DeckMovementComponent : IDeckComponent
    {
        private DeckAgent _agent;
        private NavMeshAgent _navMeshAgent;

        public void Initialize(DeckAgent deckCoreAgent)
        {
            _agent = deckCoreAgent;
            _navMeshAgent = _agent.GetComponent<NavMeshAgent>();
            DeckEventManager.Register<DeckOnNavMeshPositionSelection>(SetDestination);
        }

        public void DeInitialize()
        {
            DeckEventManager.Unregister<DeckOnNavMeshPositionSelection>(SetDestination);
        }

        public void SetDestination(DeckOnNavMeshPositionSelection obj)
        {
            _navMeshAgent.SetDestination(obj.position);
        }

        public DeckAgent GetAgent()
        {
            return _agent;
        }
    }
}