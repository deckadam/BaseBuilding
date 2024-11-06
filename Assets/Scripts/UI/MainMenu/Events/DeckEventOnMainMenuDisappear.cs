using Deck.EventManager;

namespace Deck.Components.Building
{
    public class DeckEventOnMainMenuDisappear : IDeckEvent
    {
        public static DeckEventOnMainMenuDisappear Create()
        {
            return new DeckEventOnMainMenuDisappear();
        }
    }
}