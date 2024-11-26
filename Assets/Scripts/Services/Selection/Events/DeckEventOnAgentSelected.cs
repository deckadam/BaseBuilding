using Deck.Base;
using Deck.EventManager;

namespace Deck.Services.Selection.Events
{
    public class DeckEventOnAgentSelected : IDeckEvent
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