using Deck.EventManager;

namespace Deck.Components.Building
{
    public class DeckEventOnGameSceneLoaded : IDeckEvent
    {
        public static DeckEventOnGameSceneLoaded Create()
        {
            return new DeckEventOnGameSceneLoaded();
        }
    }
}