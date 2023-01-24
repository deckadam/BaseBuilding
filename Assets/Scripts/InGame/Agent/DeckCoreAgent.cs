using Cysharp.Threading.Tasks;
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
        private void Inject(DeckDataAgent dataAgent, IDeckComponent[] components)
        {
            _data = dataAgent;

            SetComponents(components);
        }

        public void LoadData(string id)
        {
            _data.Initialize();
            Initialize();
            GetDeckComponent<DeckInventoryComponent>().Initialize(this);
            _id = id;
            Debug.LogError("Load data");
        }

        public void LoadData(DeckComponentHolderSaveData data)
        {
            LoadData(data.id);
            LoadComponentData(data.componentDatas);
        }

        private void OnDestroy()
        {
            DeInitialize();
            OnPossessionEnd();
        }

        private void Update()
        {
            Tick();
        }

        public override async void RequestDeath()
        {
            DeckOnComponentHolderDeath.Create(this).Send();
            await UniTask.NextFrame();
            DeInitialize();
            _memory.Despawn(this);
        }

        public override void OnPossessionStart()
        {
            GetDeckComponent<DeckMovementComponent>().StartTracking();
        }

        public override void OnPossessionEnd()
        {
            GetDeckComponent<DeckMovementComponent>().StopTracking();
        }

        public void OnSpawned(IMemoryPool p1)
        {
            _memory = p1;
            transform.SetParent(DeckMapService.map.transform);
            transform.localPosition = Vector3.zero;
            Services.Deck.GetService<DeckMapService>().AddCoreAgent(this);
        }

        public void OnDespawned()
        {
            Services.Deck.GetService<DeckMapService>().RemoveCoreAgent(this);
        }

        public override T GetData<T>()
        {
            return _data.GetData<T>();
        }

        public override string GetName() => _id;

        public class Factory : PlaceholderFactory<DeckCoreAgent>
        {
        }
    }
}