using Cysharp.Threading.Tasks;
using Deck.Services.Building;
using Deck.Utility;
using InGame.Agent.Building;
using Services.AreaController.Events;
using Services.Building;
using Utility;

namespace Deck.InGame.Agent.Door
{
    public class DeckAgentAgentDoor : DeckAgentBuilding
    {
        protected override async void InternalAfterBuildingInitialized()
        {
            await UniTask.Yield();
            DeckEventOnDoorBuild.Create(transform.position.ToVector2Int()).Send();
            global::Services.DeckServiceProvider.GetService<DeckServiceBuilding>().SetCellsOccupied(transform.position.ToVector2Int(), BuildingData.Indices, this);
        }

        protected override void OnAgentDestroyed()
        {
            var cellPosition = transform.position.ToVector2Int();
            DeckEventOnDoorDestroyed.Create(cellPosition).Send();
            global::Services.DeckServiceProvider.GetService<DeckServiceBuilding>().SetCellsUnoccupied(transform.position.ToVector2Int(), BuildingData.Indices);
        }
    }
}