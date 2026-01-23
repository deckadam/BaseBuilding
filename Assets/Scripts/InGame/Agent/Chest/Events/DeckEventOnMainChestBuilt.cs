using EventManager;

namespace InGame.Agent.Chest.Events
{
    public struct DeckEventOnMainChestBuilt : IDeckEvent
    {
        public DeckBuildingChest chest;

        public static DeckEventOnMainChestBuilt Create(DeckBuildingChest chest)
        {
            return new DeckEventOnMainChestBuilt()
            {
                chest = chest
            };
        }
    }
}