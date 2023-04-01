using Cysharp.Threading.Tasks;
using Deck.Components;
using Deck.Data;
using Deck.Events;
using Deck.Events.MapService;
using Deck.InputHandling.Events;
using Deck.Utility.Logger;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace Deck
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class DeckAgentCore : DeckAgent
    {
        [Inject]
        private void Inject(DeckComponent[] injectedComponents)
        {
            SetComponents(injectedComponents);
        }

        private void Update()
        {
            foreach (var deckComponent in components)
            {
                deckComponent.Tick();
            }
        }

        protected override void InternalRequestDeath()
        {
            DeckOnCoreAgentDeathEvent.Create(this).Send();
            Destroy(gameObject);
        }

        private void OnEnable()
        {
            transform.SetParent(DeckServiceMap.GetMap().transform, true);
            DeckLogger.Level("Adding player");
            DeckOnCoreAgentCreatedEvent.Create(this).Send();
        }
    }
}