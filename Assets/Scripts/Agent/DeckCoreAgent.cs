using Cysharp.Threading.Tasks;
using Deck.Components;
using Deck.Data.Agent;
using Deck.Generalnterfaces;
using Deck.Inventory;
using Zenject;

namespace Deck.Player
{
    public class DeckCoreAgent : DeckAgent, IDeckInventoryHolder
    {
        private DeckAgentData _agentData;
        private DeckMovementComponent _movementComponent;
        private DeckHealthComponent _healthComponent;
        private DeckInventory _inventory;
        private DeckDamageDealerComponent _damageDealerComponent;

        [Inject]
        private void Inject(DeckAgentData agentData, DeckMovementComponent movementComponent, DeckHealthComponent healthComponent, DeckInventory inventory, DeckDamageDealerComponent damageDealerComponent)
        {
            _agentData = agentData;
            _movementComponent = movementComponent;
            _healthComponent = healthComponent;
            _inventory = inventory;
            _damageDealerComponent = damageDealerComponent;
        }

        private void OnEnable()
        {
            _inventory.Initialize();

            _healthComponent.SetData(_agentData.healthData);
            _healthComponent.Initialize(this);

            _movementComponent.Initialize(this);
        }

        private void OnDisable()
        {
            _movementComponent.DeInitialize();
            _healthComponent.DeInitialize();
        }

        public override async void Die()
        {
            await UniTask.NextFrame();
            OnDisable();
            Destroy(gameObject);
        }

        public DeckInventory GetInventory() => _inventory;
    }
}