using Deck.EventManager;

namespace Deck.UI.MainMenu.Events
{
    public class DeckEventOnMainMenuAppeared : IDeckEvent
    {
        public static DeckEventOnMainMenuAppeared Create()
        {
            return new DeckEventOnMainMenuAppeared();
        }
    }
}