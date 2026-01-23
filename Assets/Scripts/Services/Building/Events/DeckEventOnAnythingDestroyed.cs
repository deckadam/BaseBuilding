using EventManager;

namespace Services.Building.Events
{
    public struct DeckEventOnAnythingDestroyed:IDeckEvent
    {
        public static DeckEventOnAnythingDestroyed Create()
        {
            return new DeckEventOnAnythingDestroyed();
        }
    }
}