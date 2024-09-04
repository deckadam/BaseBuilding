using Deck.EventManager;

namespace Deck.Components.Building
{
    public class DeckEventOnMainMenuDisappear : DeckEvent
    {
        public static DeckEventOnMainMenuDisappear Create()
        {
            return new DeckEventOnMainMenuDisappear();
        }
    }
}