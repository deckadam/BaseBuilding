using Deck.EventManager;

namespace Deck.UI
{
    public class DeckOnMainMenuDisappearEvent : DeckEvent
    {
        public static DeckOnMainMenuDisappearEvent Create()
        {
            return new DeckOnMainMenuDisappearEvent();
        }
    }
}