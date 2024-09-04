using Deck.EventManager;

namespace Deck.InGame.Agent.Building
{
    public class DeckEventOnMainMenuAppeared : DeckEvent
    {
        public static DeckEventOnMainMenuAppeared Create()
        {
            return new DeckEventOnMainMenuAppeared();
        }
    }
}