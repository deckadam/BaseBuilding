using Cysharp.Threading.Tasks;
using Deck.Components;
using Deck.Data.Agent;
using Deck.Services.Implementations.CellSelectionService.Events;
using Deck.Services.Implementations.MapService;
using Deck.Utility.Logger;
using UnityEngine;
using Zenject;

namespace Deck.Agent
{
    public class DeckAgentCore : DeckAgent
    {
        [Inject]
        private void Inject(DeckDataAgentCore data, DeckComponent[] components)
        {
            SetComponentDatas(data.GetDataArray());
            Debug.LogError(components.Length);
            SetComponents(components);
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