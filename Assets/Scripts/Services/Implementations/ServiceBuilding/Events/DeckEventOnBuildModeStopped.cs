using Deck.EventManager;

namespace Deck.Services.Building
{
    public struct DeckEventOnBuildModeStopped:IDeckEvent
    {
        public static DeckEventOnBuildModeStopped Create()
        {
            return new DeckEventOnBuildModeStopped();
        }
    }
}