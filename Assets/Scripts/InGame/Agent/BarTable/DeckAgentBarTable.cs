using Cysharp.Threading.Tasks;
using InGame.Agent.Building;
using Services;
using Services.Building;
using Services.Building.Buildable.Data.Parameter.Implementations.Build;
using Utility;

namespace InGame.Agent.BarTable
{
    public class DeckAgentBarTable : DeckAgentBuilding
    {
        private DeckGridBasedData _data;

        protected override async void InternalAfterBuildingInitialized()
        {
            await UniTask.Yield();
            _data = BuildingData.GetParameter<DeckBuildableParameterBuildModeGridBased>().GetValue<DeckGridBasedData>();
            DeckServiceProvider.GetService<DeckServiceBuilding>().SetCellsOccupied(GetPosition().ToVector2Int(), _data.Indices, this);
        }

        protected override void OnAgentDestroyed()
        {
            DeckServiceProvider.GetService<DeckServiceBuilding>().SetCellsUnoccupied(GetPosition().ToVector2Int(), _data.Indices);
        }
    }
}