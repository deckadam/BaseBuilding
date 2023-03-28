using Deck.Components;
using Deck.Data;
using UnityEngine;
using Zenject;

namespace Deck
{
    public class DeckAgentChest : DeckAgent
    {
        [SerializeField] private DeckDataAgentChest chestData;

        [Inject]
        private void Inject(DeckComponent[] components)
        {
            SetComponentDatas(chestData.GetDataArray());
            SetComponents(components);
        }

        protected override void InternalRequestDeath()
        {
            Destroy(gameObject);
        }
    }
}