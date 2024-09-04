using Deck.EventManager;

namespace Deck.Components.Building
{
    public class DeckEventOnGameSceneLoaded : DeckEvent
    {
        public static DeckEventOnGameSceneLoaded Create()
        {
            return new DeckEventOnGameSceneLoaded();
        }
    }
}