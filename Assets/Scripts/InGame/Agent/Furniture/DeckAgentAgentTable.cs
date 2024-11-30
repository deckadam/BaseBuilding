using Deck.InGame.Agent.Building;
using Deck.Services.Tables.Events;
using Deck.Utility;

namespace Deck.InGame.Agent.Furniture
{
    public class DeckAgentAgentTable : DeckAgentBuilding
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