using EventManager;
using InGame.Agent.Building;

namespace Services.Building.Events
{
    public struct DeckEventOnAnythingBuilt : IDeckEvent
    {
        public DeckAgentBuilding building;

        public static DeckEventOnAnythingBuilt Create(DeckAgentBuilding building)
        {
            return new DeckEventOnAnythingBuilt()
            {
                building = building
            };
        }
    }
}