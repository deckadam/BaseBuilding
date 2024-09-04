using Deck.EventManager;
using Deck.Components;

namespace Deck.Services
{
    public class DeckEventOnAgentPossessed : DeckEvent
    {
        public DeckAgent agent { get; private set; }

        public static DeckEventOnAgentPossessed Crate(DeckAgent agent)
        {
            return new DeckEventOnAgentPossessed
            {
                agent = agent
            };
        }
    }
}