using Cysharp.Threading.Tasks;
using Deck.Components;
using Deck.Data.Agent;
using Deck.Generalnterfaces;
using Deck.Inventory;
using Deck.SaveService.Data;
using Deck.Test.General;
using Deck.Test.Markers;
using UnityEngine;
using Zenject;

namespace Deck.Player
{
    public class DeckCoreAgent : DeckAgent, IDeckInventoryHolder, IDeckDamagable
    {
        [SerializeField] private string _id;

        [Inject]
        private void Inject(DeckAgentData agentData, IDeckComponent[] components)
        {
            _data = agentData;

            SetComponents(components);
        }

        public void LoadData(string id)
        {
            _data.Initialize();
            Initialize();
            GetDeckComponent<DeckInventoryComponent>().Initialize(this);
            _id = id;
        }

        public void LoadData(DeckCoreAgentSaveData data)
        {
            LoadData(data.id);
            LoadComponentData(data.componentDatas);
        }

        private void OnDisable()
        {
            DeInitialize();
        }

        private void Update()
        {
            Tick();
        }

        public override async void RequestDeath()
        {
            await UniTask.NextFrame();
            DeInitialize();
            Destroy(gameObject);
        }

        public override T GetData<T>()
        {
            return _data.GetData<T>();
        }

        public override void OnPossesStarted()
        {
            GetDeckComponent<DeckMovementComponent>().StartTracking();
        }

        public override void OnPossesFinished()
        {
            GetDeckComponent<DeckMovementComponent>().StopTracking();
        }

        public override DeckSelectOperations[] GetAvailableOperations()
        {
            return new[]
            {
                DeckSelectOperations.posess,
                DeckSelectOperations.showInventory,
                DeckSelectOperations.takeDamage
            };
        }

        public override DeckInventoryComponent GetInventory() => GetDeckComponent<DeckInventoryComponent>();
        public override DeckHealthComponent GetHealthComponent() => GetDeckComponent<DeckHealthComponent>();
        public override string GetName() => _id;
    }
}