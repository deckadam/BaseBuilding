using Cysharp.Threading.Tasks;
using InGame.Agent.Building;
using Services;
using Services.AreaController.Events;
using Services.Building;
using Services.Building.Buildable.Data.Parameter.Implementations.Build;
using Utility;

namespace InGame.Agent.Door
{
    public class DeckAgentDoor : DeckAgentBuilding
    {
        private DeckGridBasedData _data;

        protected override async void InternalAfterBuildingInitialized()
        {
            await UniTask.Yield();
            DeckEventOnDoorBuild.Create(transform.position.ToVector2Int()).Send();
            _data = BuildingData.GetParameter<DeckBuildableParameterBuildModeGridBased>().GetValue<DeckGridBasedData>();
            DeckServiceProvider.GetService<DeckServiceBuilding>().SetCellsOccupied(GetPosition().ToVector2Int(), _data.Indices, this);
        }

        protected override void OnAgentDestroyed()
        {
            var cellPosition = GetPosition().ToVector2Int();
            DeckEventOnDoorDestroyed.Create(cellPosition).Send();
            DeckServiceProvider.GetService<DeckServiceBuilding>().SetCellsUnoccupied(cellPosition, _data.Indices);
        }
    }
}