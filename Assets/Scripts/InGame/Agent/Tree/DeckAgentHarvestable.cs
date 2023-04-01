using Deck.Components;
using Zenject;

namespace Deck
{
    public class DeckAgentHarvestable : DeckAgent
    {
        [Inject]
        private void Inject(DeckComponent[] components)
        {
            SetComponents(components);
        }
    }
}