using Cysharp.Threading.Tasks;
using Deck.Component;
using Deck.Components;
using Deck.Data.Agent;
using Deck.Inventory;
using Deck.Save.Data;
using Deck.Services.Implementations.CellSelectionService.Events;
using Deck.Services.Implementations.MapService;
using Deck.Utility.Logger;
using UnityEngine;
using Zenject;

namespace Deck.Agent
{
    public class DeckCoreAgent : DeckAgent, IPoolable<IMemoryPool>
    {
        [SerializeField] private string _id;
        private IMemoryPool _memory;

        [Inject]
        private void Inject(DeckDataAgent data, DeckComponent[] components)
        {
            SetComponentDatas(data.GetDataArray());
            SetComponents(components);
        }

        public void LoadData(string id)
        {
            Initialize();
            GetDeckComponent<DeckInventoryComponent>().Initialize(this);
            _id = id;
        }

        public void LoadData(DeckComponentHolderSaveData data)
        {
            LoadData(data.id);
            LoadComponentData(data.componentDatas);
        }

        public override async void RequestDeath()
        {
            DeckOnAgentDeathEvent.Create(this).Send();
            await UniTask.NextFrame();
            DeInitialize();
            _memory.Despawn(this);
        }

        public void OnSpawned(IMemoryPool p1)
        {
            _memory = p1;
            transform.SetParent(DeckMapService.map.transform);
            transform.localPosition = Vector3.zero;
            Deck.GetService<DeckMapService>().AddCoreAgent(this);
        }

        public void OnDespawned()
        {
            Deck.GetService<DeckMapService>().RemoveCoreAgent(this);
        }

        public override string GetName() => _id;

        public class Factory : PlaceholderFactory<DeckCoreAgent>
        {
        }
    }
}