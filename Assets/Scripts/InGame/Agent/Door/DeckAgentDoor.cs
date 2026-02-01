using Cysharp.Threading.Tasks;
using Data.Buildable.Data.Parameter.Implementations.Build;
using InGame.Agent.Building;
using Services;
using Services.AreaController.Events;
using Services.Building;
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
            DeckServiceProvider.GetService<DeckServiceBuilding>().SetCellsOccupied(transform.position.ToVector2Int(), _data.Indices, this);
        }

        protected override void OnAgentDestroyed()
        {
            var cellPosition = transform.position.ToVector2Int();
            DeckEventOnDoorDestroyed.Create(cellPosition).Send();
            DeckServiceProvider.GetService<DeckServiceBuilding>().SetCellsUnoccupied(transform.position.ToVector2Int(), _data.Indices);
        }
    }
}