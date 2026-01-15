using EventManager;

namespace UI.MainMenu.Events
{
    public class DeckEventOnGameSceneLoaded : IDeckEvent
    {
        public static DeckEventOnGameSceneLoaded Create()
        {
            return new DeckEventOnGameSceneLoaded();
        }
    }
}