using Cysharp.Threading.Tasks;
using InGame.Agent.Building;
using Services;
using Services.Building;
using Utility;

namespace InGame.Agent.BarTable
{
    public class DeckAgentAgentBarTable : DeckAgentBuilding
    {
        protected override async void InternalAfterBuildingInitialized()
        {
            await UniTask.Yield();
            DeckServiceProvider.GetService<DeckServiceBuilding>().SetCellsOccupied(transform.position.ToVector2Int(), BuildingData.Indices, this);
        }

        protected override void OnAgentDestroyed()
        {
            DeckServiceProvider.GetService<DeckServiceBuilding>().SetCellsUnoccupied(transform.position.ToVector2Int(), BuildingData.Indices);
        }
    }
}