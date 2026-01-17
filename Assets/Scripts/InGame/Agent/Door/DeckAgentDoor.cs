using Cysharp.Threading.Tasks;
using InGame.Agent.Building;
using Services;
using Services.AreaController.Events;
using Services.Building;
using Utility;

namespace InGame.Agent.Door
{
    public class DeckAgentDoor : DeckAgentBuilding
    {
        protected override async void InternalAfterBuildingInitialized()
        {
            await UniTask.Yield();
            DeckEventOnDoorBuild.Create(transform.position.ToVector2Int()).Send();
            DeckServiceProvider.GetService<DeckServiceBuilding>().SetCellsOccupied(transform.position.ToVector2Int(), BuildingData.Indices, this);
        }

        protected override void OnAgentDestroyed()
        {
            var cellPosition = transform.position.ToVector2Int();
            DeckEventOnDoorDestroyed.Create(cellPosition).Send();
            DeckServiceProvider.GetService<DeckServiceBuilding>().SetCellsUnoccupied(transform.position.ToVector2Int(), BuildingData.Indices);
        }
    }
}