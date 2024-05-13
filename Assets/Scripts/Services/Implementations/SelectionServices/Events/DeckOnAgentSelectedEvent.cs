using Deck.Agent;
using Deck.EventManager;

namespace Deck.Services
{
    public class DeckOnAgentSelectedEvent : DeckEvent
    {
        public DeckAgent agent { get; private set; }

        public static DeckOnAgentSelectedEvent Create(DeckAgent agent)
        {
            return new()
            {
                agent = agent
            };
        }
    }
}