using Deck.EventManager;
using Deck.Components;

namespace Deck.Services
{
    public class DeckEventOnAgentReleased : IDeckEvent
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