using Deck.Component;
using Deck.EventManager;

namespace Deck.Services.Implementations.CellSelectionService.Events
{
    public class DeckOnAgentPossessedEvent : DeckEvent
    {
        public DeckAgent agent { get; private set; }

        public static DeckOnAgentPossessedEvent Crate(DeckAgent agent)
        {
            return new()
            {
                agent = agent
            };
        }
    }
}