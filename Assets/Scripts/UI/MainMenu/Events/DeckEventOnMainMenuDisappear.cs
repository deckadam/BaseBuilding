using Deck.EventManager;

namespace Deck.UI
{
    public class DeckEventOnMainMenuDisappear : DeckEvent
    {
        public static DeckEventOnMainMenuDisappear Create()
        {
            return new DeckEventOnMainMenuDisappear();
        }
    }
}