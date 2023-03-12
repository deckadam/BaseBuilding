using Deck.Agent;
using Deck.Components;
using Deck.Data.Agent;
using Zenject;

namespace Deck.Map.Agent.Chest
{
    public class DeckAgentChest : DeckAgent
    {
        [Inject]
        private void Inject(DeckDataAgentChest chestData, DeckComponent[] components)
        {
            SetComponentDatas(chestData.GetDataArray());
            SetComponents(components);
        }

        public override void RequestDeath()
        {
            Destroy(gameObject);
        }

        public class Factory : PlaceholderFactory<DeckAgentChest>
        {
        }
    }
}