using EventManager;

namespace Deck.Services.Building.Events
{
    public struct DeckEventOnBuildModeStopped:IDeckEvent
    {
        public static DeckEventOnBuildModeStopped Create()
        {
            return new DeckEventOnBuildModeStopped();
        }
    }
}