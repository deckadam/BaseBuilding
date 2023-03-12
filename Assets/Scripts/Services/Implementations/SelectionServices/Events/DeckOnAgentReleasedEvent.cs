using Deck.Agent;
using Deck.EventManager;

namespace Deck.Services.Implementations.CellSelectionService.Events
{
    public class DeckOnAgentReleasedEvent : DeckEvent
    {
        public DeckAgent agent { get; private set; }

        public static DeckOnAgentReleasedEvent Create(DeckAgent agent)
        {
            return new()
            {
                agent = agent
            };
        }
    }
}