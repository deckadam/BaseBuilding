using EventManager;

namespace Services.Building.Events
{
    public struct DeckEventOnBuildModeStopped:IDeckEvent
    {
        public static DeckEventOnBuildModeStopped Create()
        {
            return new DeckEventOnBuildModeStopped();
        }
    }
}