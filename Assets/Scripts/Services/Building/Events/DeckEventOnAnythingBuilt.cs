using Base;
using EventManager;

namespace Services.Building.Events
{
    public struct DeckEventOnAnythingBuilt : IDeckEvent
    {
        public DeckAgent building;

        public static DeckEventOnAnythingBuilt Create(DeckAgent building)
        {
            return new DeckEventOnAnythingBuilt()
            {
                building = building
            };
        }
    }
}