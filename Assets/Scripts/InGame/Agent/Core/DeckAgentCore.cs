using Cysharp.Threading.Tasks;
using Deck.Components;
using Deck.Data.Agent;
using Deck.Services.Implementations.CellSelectionService.Events;
using Deck.Services.Implementations.MapService;
using Deck.Utility.Logger;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace Deck.Agent
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class DeckAgentCore : DeckAgent
    {
        [Inject]
        private void Inject(DeckDataAgentCore data, DeckComponent[] injectedComponents)
        {
            SetComponentDatas(data.GetDataArray());
            SetComponents(injectedComponents);
        }

        public override async void RequestDeath()
        {
            DeckOnAgentDeathEvent.Create(this).Send();
            await UniTask.NextFrame();
            DeInitialize();
            Deck.GetService<DeckMapService>().RemoveCoreAgent(this);
        }

        private void OnEnable()
        {
            transform.SetParent(DeckMapService.map.transform);
            transform.localPosition = Vector3.zero;
            Deck.GetService<DeckMapService>().AddCoreAgent(this);
        }

        public class Factory : PlaceholderFactory<DeckAgentCore>
        {
        }
    }
}