using EventManager;

namespace Services.Raid.Events
{
    public struct DeckEventOnRaidEnded : IDeckEvent
    {
        public static DeckEventOnRaidEnded Create()
        {
            return new DeckEventOnRaidEnded();
        }
    }
}