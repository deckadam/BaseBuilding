using Deck.Base;
using Deck.EventManager;

namespace Deck.Services.Selection.Events
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