using Cysharp.Threading.Tasks;
using Deck.InGame.Agent.Building;
using Deck.Services.Building;
using Deck.Utility;
using UnityEngine;

namespace Deck.InGame.Agent.BarTable
{
    public class DeckAgentAgentBarTable : DeckAgentBuilding
    {
        protected override async void InternalAfterBuildingInitialized()
        {
            await UniTask.Yield();
            Deck.GetService<DeckServiceBuilding>().SetCellOccupied(transform.position.ToVector2Int(), BuildingData.Indices, this);
        }

        protected override void OnAgentDestroyed()
        {
            Deck.GetService<DeckServiceBuilding>().SetCellsUnoccupied(transform.position.ToVector2Int(), BuildingData.Indices);
        }
    }
}