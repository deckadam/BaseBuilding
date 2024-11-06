using Deck.EventManager;

namespace Deck.Components.Building
{
    public class DeckEventOnMainMenuAppeared : IDeckEvent
    {
        public static DeckEventOnMainMenuAppeared Create()
        {
            return new DeckEventOnMainMenuAppeared();
        }
    }
}