using EventManager;

namespace UI.MainMenu.Events
{
    public struct DeckEventOnMainMenuAppeared : IDeckEvent
    {
        public static DeckEventOnMainMenuAppeared Create()
        {
            return new DeckEventOnMainMenuAppeared();
        }
    }
}