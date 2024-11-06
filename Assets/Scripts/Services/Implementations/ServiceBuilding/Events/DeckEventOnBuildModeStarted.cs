using Deck.EventManager;

namespace Deck.Services.Building
{
    public struct DeckEventOnBuildModeStarted:IDeckEvent
    {
        public static DeckEventOnBuildModeStarted Create()
        {
            return new DeckEventOnBuildModeStarted();
        }
    }
}