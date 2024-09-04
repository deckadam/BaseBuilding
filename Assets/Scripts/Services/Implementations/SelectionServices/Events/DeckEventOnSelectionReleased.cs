using Deck.EventManager;
using Deck.InGame.Agent;

namespace Deck.Services
{
    public class DeckEventOnSelectionReleased : DeckEvent
    {
        public DeckAgent agent { get; private set; }

        public static DeckEventOnSelectionReleased Create(DeckAgent agent)
        {
            return new()
            {
                agent = agent
            };
        }
    }
}