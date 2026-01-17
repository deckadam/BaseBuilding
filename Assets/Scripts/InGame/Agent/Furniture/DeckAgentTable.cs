using InGame.Agent.Building;
using Services.Tables.Events;
using Utility;

namespace InGame.Agent.Furniture
{
    public class DeckAgentTable : DeckAgentBuilding
    {
        protected override void InternalAfterBuildingInitialized()
        {
            DeckEventOnTablePlaced.Create(this).Send();
        }

        protected override void OnAgentDestroyed()
        {
            DeckEventOnTableDestroyed.Create(this).Send();
        }
    }
}