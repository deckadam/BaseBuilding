using Cysharp.Threading.Tasks;
using InGame.Agent.Building;
using Services;
using Services.AreaController.Events;
using Services.Building;
using Services.Building.Buildable.Data.Parameter.Implementations.Build;
using Utility;

namespace InGame.Agent.Wall
{
    public class DeckAgentWall : DeckAgentBuilding
    {
        private DeckGridBasedData _data;

        protected override async void InternalAfterBuildingInitialized()
        {
            await UniTask.Yield();
            DeckEventOnWallBuild.Create(transform.position.ToVector2Int()).Send();
            _data = BuildingData.GetParameter<DeckBuildableParameterBuildModeGridBased>().GetValue<DeckGridBasedData>();
            DeckServiceProvider.GetService<DeckServiceBuilding>().SetCellsOccupied(transform.position.ToVector2Int(), _data.Indices, this);
        }

        protected override void OnAgentDestroyed()
        {
            DeckEventOnWallDestroyed.Create(transform.position.ToVector2Int()).Send();
            DeckServiceProvider.GetService<DeckServiceBuilding>().SetCellsUnoccupied(transform.position.ToVector2Int(), _data.Indices);
        }
    }
}