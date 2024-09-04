using Deck.EventManager;

namespace Deck.Components.Building
{
    public class DeckEventOnMainMenuAppeared : DeckEvent
    {
        public static DeckEventOnMainMenuAppeared Create()
        {
            return new DeckEventOnMainMenuAppeared();
        }
    }
}