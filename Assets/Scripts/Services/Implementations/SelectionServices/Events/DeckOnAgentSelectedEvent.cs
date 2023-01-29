using Deck.Component;
using Deck.EventManager;

namespace Deck.Services.Implementations.CellSelectionService.Events
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