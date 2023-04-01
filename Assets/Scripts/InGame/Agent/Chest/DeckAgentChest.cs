using Deck.Components;
using Zenject;

namespace Deck
{
    public class DeckAgentChest : DeckAgent
    {
        [Inject]
        private void Inject(DeckComponent[] components)
        {
            SetComponents(components);
        }
    }
}