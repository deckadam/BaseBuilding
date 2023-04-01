using Deck.Components;
using Zenject;

namespace Deck
{
    public class DeckAgentWall : DeckAgent
    {
        [Inject]
        private void Inject(DeckComponent[] injectedComponents)
        {
            SetComponents(injectedComponents);
        }
    }
}