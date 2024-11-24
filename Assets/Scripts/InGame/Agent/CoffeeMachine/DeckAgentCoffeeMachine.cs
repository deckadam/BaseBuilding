using Deck.Components.Building;
using Deck.Services.Building;
using Deck.Utility;
using UnityEngine;

namespace Deck.InGame.Agent.CoffeeMachine
{
    public class DeckAgentCoffeeMachine : DeckBuilding
    {
        protected override void AfterInitialize()
        {
            var rotationCount = Mathf.RoundToInt(transform.localRotation.eulerAngles.y / 90f) % 4;
            var rotatedIndices = BuildingData.AccessIndices.GetRotatedIndices(rotationCount);
            Deck.GetService<DeckServiceBuilding>().AddAccessCellReference(transform.position.ToVector2Int(), rotatedIndices);
        }

        protected override void OnAgentDestroyed()
        {
            var cellIndex = transform.position.ToVector2Int();
            var rotationCount = Mathf.RoundToInt(transform.localRotation.eulerAngles.y / 90f) % 4;
            
            var accessIndices = BuildingData.AccessIndices.GetRotatedIndices(rotationCount);
            Deck.GetService<DeckServiceBuilding>().RemoveAccessCellReference(cellIndex, accessIndices);
        }
    }
}