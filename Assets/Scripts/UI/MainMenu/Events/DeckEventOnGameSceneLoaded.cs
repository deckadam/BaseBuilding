using EventManager;

namespace UI.MainMenu.Events
{
    public struct DeckEventOnGameSceneLoaded : IDeckEvent
    {
        public static DeckEventOnGameSceneLoaded Create()
        {
            return new DeckEventOnGameSceneLoaded();
        }
    }
}