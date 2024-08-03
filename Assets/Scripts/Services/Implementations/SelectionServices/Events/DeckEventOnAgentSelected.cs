using Deck.Agent;
using Deck.EventManager;

namespace Deck.Services
{
    public class DeckEventOnAgentSelected : DeckEvent
    {
        public DeckAgent agent { get; private set; }

        public static DeckEventOnAgentSelected Create(DeckAgent agent)
        {
            return new()
            {
                agent = agent
            };
        }
    }
}