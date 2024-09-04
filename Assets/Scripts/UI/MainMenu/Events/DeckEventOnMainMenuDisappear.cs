using Deck.EventManager;

namespace Deck.InGame.Agent.Building
{
    public class DeckEventOnMainMenuDisappear : DeckEvent
    {
        public static DeckEventOnMainMenuDisappear Create()
        {
            return new DeckEventOnMainMenuDisappear();
        }
    }
}