using Deck.Components;
using Zenject;

namespace Deck.UI.InGame.Agent.Enemy
{
    public class DeckAgentEnemy : DeckAgent
    {
        [Inject]
        private void Inject(DeckComponent[] injectedComponents)
        {
            SetComponents(injectedComponents);
        }

        private void Update()
        {
            foreach (var deckComponent in components)
            {
                deckComponent.Tick();
            }
        }

        protected override void InternalRequestDeath()
        {
            Destroy(gameObject);
        }
    }
}