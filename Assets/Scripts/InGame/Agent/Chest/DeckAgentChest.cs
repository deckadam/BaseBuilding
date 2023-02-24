using Deck.Component;
using Deck.Components;
using Deck.Data.Agent;
using Deck.Inventory;
using Zenject;

namespace Deck.Map.Agent.Chest
{
    public class DeckAgentChest : DeckAgent
    {
        [Inject]
        private void Inject(DeckDataAgentChest chestData, DeckInventoryComponent inventoryComponent,
            DeckHealthComponent healthComponent)
        {
            SetComponentDatas(chestData.GetDataArray());
            SetComponents(inventoryComponent, healthComponent);
        }

        public override void RequestDeath()
        {
            Destroy(gameObject);
        }

        public override string GetName()
        {
            return null;
        }

        public class Factory : PlaceholderFactory<DeckAgentChest>
        {
        }
    }
}