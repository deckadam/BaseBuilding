using Deck.Agent;
using Deck.EventManager;

namespace Deck.Services
{
    public class DeckEventOnAgentReleased : DeckEvent
    {
        public DeckAgent agent { get; private set; }

        public static DeckEventOnAgentReleased Create(DeckAgent agent)
        {
            return new DeckEventOnAgentReleased
            {
                agent = agent
            };
        }
    }
}