using EventManager;

namespace UI.MainMenu.Events
{
    public struct DeckEventOnMainMenuDisappear : IDeckEvent
    {
        public static DeckEventOnMainMenuDisappear Create()
        {
            return new DeckEventOnMainMenuDisappear();
        }
    }
}