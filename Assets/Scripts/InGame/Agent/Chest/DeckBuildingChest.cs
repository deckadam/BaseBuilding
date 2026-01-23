using InGame.Agent.Building;
using InGame.Agent.Chest.Events;
using Utility;

namespace InGame.Agent.Chest
{
    public class DeckBuildingChest : DeckAgentBuilding
    {
        protected override void InternalAfterBuildingInitialized()
        {
            DeckEventOnMainChestBuilt.Create(this).Send();
        }
    }
}