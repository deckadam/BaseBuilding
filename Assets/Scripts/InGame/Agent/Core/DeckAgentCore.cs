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
        [SerializeField] private DeckDataAgentCore data;

        [Inject]
        private void Inject(DeckComponent[] injectedComponents)
        {
            SetComponentDatas(data.GetDataArray());
            SetComponents(injectedComponents);
        }

        private void Update()
        {
            foreach (var deckComponent in components)
            {
                deckComponent.Tick();
            }
        }

        protected override async void InternalRequestDeath()
        {
            DeckOnCoreAgentDeathEvent.Create(this).Send();
            await UniTask.NextFrame();
            DeInitialize();
            DeckLogger.Level("Removing player");
            Destroy(gameObject);
        }

        private void OnEnable()
        {
            transform.SetParent(DeckServiceMap.GetMap().transform);
            transform.localPosition = Vector3.zero;
            DeckLogger.Level("Adding player");
            DeckOnCoreAgentCreatedEvent.Create(this).Send();
        }
    }
}