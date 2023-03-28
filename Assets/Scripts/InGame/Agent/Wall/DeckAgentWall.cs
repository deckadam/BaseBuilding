using Data.Component;
using Deck.Components;
using Deck.Data.Component;
using UnityEngine;
using Zenject;

namespace Deck
{
    public class DeckAgentWall : DeckAgent
    {
        [SerializeField] private DeckDataHealth healthData;

        [Inject]
        private void Inject(DeckComponent[] injectedComponents)
        {
            SetComponentDatas(new DeckDataComponent[] { healthData });
            SetComponents(injectedComponents);
        }

        protected override void InternalRequestDeath()
        {
            Destroy(gameObject);
        }
    }
}