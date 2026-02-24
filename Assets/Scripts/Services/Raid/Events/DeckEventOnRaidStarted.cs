using EventManager;

namespace Services.Raid.Events
{
    public struct DeckEventOnRaidStarted : IDeckEvent
    {
        public static DeckEventOnRaidStarted Create()
        {
            return new DeckEventOnRaidStarted();
        }
    }
}