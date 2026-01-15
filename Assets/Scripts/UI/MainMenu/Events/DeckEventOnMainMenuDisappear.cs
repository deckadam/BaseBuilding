using EventManager;

namespace Deck.UI.MainMenu.Events
{
    public class DeckEventOnMainMenuDisappear : IDeckEvent
    {
        public static DeckEventOnMainMenuDisappear Create()
        {
            return new DeckEventOnMainMenuDisappear();
        }
    }
}