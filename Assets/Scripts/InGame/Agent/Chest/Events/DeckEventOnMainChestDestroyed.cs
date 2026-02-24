using EventManager;

namespace InGame.Agent.Chest.Events
{
    public struct DeckEventOnMainChestDestroyed : IDeckEvent
    {
        public DeckBuildingChest chest { get; private set; }

        public static DeckEventOnMainChestDestroyed Create(DeckBuildingChest chest)
        {
            return new DeckEventOnMainChestDestroyed()
            {
                chest = chest
            };
        }
    }
}