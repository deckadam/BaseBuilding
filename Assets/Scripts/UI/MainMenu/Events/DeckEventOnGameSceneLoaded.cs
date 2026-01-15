using EventManager;

namespace Deck.UI.MainMenu.Events
{
    public class DeckEventOnGameSceneLoaded : IDeckEvent
    {
        public static DeckEventOnGameSceneLoaded Create()
        {
            return new DeckEventOnGameSceneLoaded();
        }
    }
}