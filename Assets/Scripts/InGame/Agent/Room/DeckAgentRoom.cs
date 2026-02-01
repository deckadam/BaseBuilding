using Cysharp.Threading.Tasks;
using Data.Buildable.Data.Parameter.Implementations.Build;
using InGame.Agent.Building;
using Services;
using Services.Building;
using Utility;

namespace InGame.Agent.Room
{
    public class DeckAgentRoom : DeckAgentBuilding
    {
        private DeckGridBasedData _data;

        protected override async void InternalAfterBuildingInitialized()
        {
            await UniTask.Yield();
            _data = BuildingData.GetParameter<DeckBuildableParameterBuildModeGridBased>().GetValue<DeckGridBasedData>();
            DeckServiceProvider.GetService<DeckServiceBuilding>().SetCellsOccupied(transform.position.ToVector2Int(), _data.Indices, this);
        }

        protected override void OnAgentDestroyed()
        {
            DeckServiceProvider.GetService<DeckServiceBuilding>().SetCellsUnoccupied(transform.position.ToVector2Int(), _data.Indices);
        }
    }
}