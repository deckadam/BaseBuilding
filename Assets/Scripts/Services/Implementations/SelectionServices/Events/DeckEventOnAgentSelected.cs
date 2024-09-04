using Deck.EventManager;
using Deck.InGame.Agent;

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