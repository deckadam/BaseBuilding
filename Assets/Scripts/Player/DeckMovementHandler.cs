using Deck.EventManager;
using Deck.InputHandling.Events;
using UnityEngine.AI;

namespace Deck.Player
{
    public class DeckMovementHandler : IDeckCorePlayerSystem
    {
        private DeckCoreAgent _deckCoreAgent;
        private NavMeshAgent _agent;

        public void Initialize(DeckCoreAgent deckCoreAgent)
        {
            _deckCoreAgent = deckCoreAgent;
            _agent = _deckCoreAgent.GetComponent<NavMeshAgent>();
            DeckEventManager.Register<DeckOnNavMeshPositionSelection>(SetDestination);
        }

        public void DeInitialize()
        {
            DeckEventManager.Unregister<DeckOnNavMeshPositionSelection>(SetDestination);
        }

        public void SetDestination(DeckOnNavMeshPositionSelection obj)
        {
            _agent.SetDestination(obj.position);
        }
    }
}