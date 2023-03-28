using Deck;
using Deck.Components;
using Deck.Data;
using UnityEngine;
using Zenject;

namespace Deck
{
    public class DeckAgentHarvestable : DeckAgent
    {
        [SerializeField] private DeckDataAgentHarvestable data;

        [Inject]
        private void Inject(DeckComponent[] components)
        {
            SetComponentDatas(data.GetDataArray());
            SetComponents(components);
        }

        protected override void InternalRequestDeath()
        {
            Destroy(gameObject);
        }
    }
}