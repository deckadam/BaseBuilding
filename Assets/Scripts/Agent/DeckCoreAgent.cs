using Deck.Components;
using Deck.Generalnterfaces;
using Deck.Inventory;
using Deck.Test.Data.Agent;
using UnityEngine;
using Zenject;

namespace Deck.Player
{
    public class DeckCoreAgent : DeckAgent, IDeckInventoryHolder
    {
        private DeckAgentData _agentData;
        private DeckMovementComponent _movementComponent;
        private DeckHealthComponent _healthComponent;
        private DeckInventory _inventory;

        [Inject]
        private void Inject(DeckAgentData agentData, DeckMovementComponent movementComponent, DeckHealthComponent healthComponent, DeckInventory inventory)
        {
            _agentData = agentData;
            _movementComponent = movementComponent;
            _healthComponent = healthComponent;
            _inventory = inventory;
        }

        private void OnEnable()
        {
            _inventory.Initialize();

            _healthComponent.SetData(_agentData.healthComponentData);
            _healthComponent.Initialize(this);

            _movementComponent.Initialize(this);
        }

        private void OnDisable()
        {
            _movementComponent.DeInitialize();
        }

        public DeckInventory GetInventory() => _inventory;
    }
}