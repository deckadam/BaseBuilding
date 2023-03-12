using Deck.Agent;
using Deck.EventManager;

namespace Deck.Services.Implementations.CellSelectionService.Events
{
    public class DeckOnAgentDeathEvent : DeckEvent
    {
        public DeckAgent agent { get; private set; }

        public static DeckOnAgentDeathEvent Create(DeckAgent agent)
        {
            return new()
            {
                agent = agent
            };
        }
    }
}