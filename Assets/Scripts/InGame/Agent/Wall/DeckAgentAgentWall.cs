using Cysharp.Threading.Tasks;
using Deck.InGame.Agent.Building;
using Deck.Services.AreaController.Events;
using Deck.Services.Building;
using Deck.Utility;

namespace Deck.InGame.Agent.Wall
{
    public class DeckAgentAgentWall : DeckAgentBuilding
    {
        protected override async void InternalAfterBuildingInitialized()
        {
            await UniTask.Yield();
            DeckEventOnWallBuild.Create(transform.position.ToVector2Int()).Send();
            Deck.GetService<DeckServiceBuilding>().SetCellOccupied(transform.position.ToVector2Int(), BuildingData.Indices, this);
        }

        protected override void OnAgentDestroyed()
        {
            DeckEventOnWallDestroyed.Create(transform.position.ToVector2Int()).Send();
            Deck.GetService<DeckServiceBuilding>().SetCellsUnoccupied(transform.position.ToVector2Int(), BuildingData.Indices);
        }
    }
}