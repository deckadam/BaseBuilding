using EventManager;
using InGame.Agent.Chest;

namespace Services.Raid.Events
{
    public struct DeckEventOnRaidStartRequested : IDeckEvent
    {
        public DeckBuildingChest mainChest { get; private set; }

        public static DeckEventOnRaidStartRequested Create(DeckBuildingChest mainChest)
        {
            return new DeckEventOnRaidStartRequested()
            {
                mainChest = mainChest
            };
        }
    }
}