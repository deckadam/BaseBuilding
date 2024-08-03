using Deck.EventManager;

namespace Deck.UI
{
    public class DeckEventOnGameSceneLoaded : DeckEvent
    {
        public static DeckEventOnGameSceneLoaded Create()
        {
            return new DeckEventOnGameSceneLoaded();
        }
    }
}