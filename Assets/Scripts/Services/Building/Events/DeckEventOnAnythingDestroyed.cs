using EventManager;
using InGame.Agent.Building;

namespace Services.Building.Events
{
    public struct DeckEventOnAnythingDestroyed : IDeckEvent
    {
        public DeckAgentBuilding building { get; private set; }

        public static DeckEventOnAnythingDestroyed Create(DeckAgentBuilding building)
        {
            return new DeckEventOnAnythingDestroyed()
            {
                building = building
            };
        }
    }
}