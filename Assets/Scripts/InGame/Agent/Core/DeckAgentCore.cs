using System.Linq;
using Cysharp.Threading.Tasks;
using Deck.Component;
using Deck.Components;
using Deck.Data.Agent;
using Deck.Inventory;
using Deck.Save.Data;
using Deck.Services.Implementations.CellSelectionService.Events;
using Deck.Services.Implementations.MapService;
using Deck.Utility.Logger;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Deck.Agent
{
    public class DeckAgentCore : DeckAgent, IPoolable<IMemoryPool>
    {
        [FormerlySerializedAs("_id"),SerializeField] private string id;
        private IMemoryPool _memory;

        [Inject]
        private void Inject(DeckDataAgentCore data,
            DeckMovementComponent movementComponent,
            DeckHealthComponent healthComponent,
            DeckComponentDamageDealer damageDealerComponent,
            DeckInventoryComponent inventoryComponent)
        {
            SetComponentDatas(data.GetDataArray());
            SetComponents(movementComponent, healthComponent, damageDealerComponent, inventoryComponent);
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

        public override string GetName() => id;

        [Button]
        private void Test()
        {
            var deckDataItems = GetDeckComponent<DeckInventoryComponent>().GetItems().ToList();
            Debug.LogError(deckDataItems.Count);
            foreach (var item in deckDataItems)
            {
                Debug.LogError(item == null);
            }
        }

        public class Factory : PlaceholderFactory<DeckAgentCore>
        {
        }
    }
}