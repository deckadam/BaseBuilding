using Deck.EventManager;

namespace Deck.UI
{
    public class DeckEventOnMainMenuAppeared : DeckEvent
    {
        public static DeckEventOnMainMenuAppeared Create()
        {
            return new DeckEventOnMainMenuAppeared();
        }
    }
}