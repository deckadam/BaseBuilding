using Deck.InGame.Agent.Building;
using Deck.Services.Building;
using Deck.Utility;
using UnityEngine;

namespace Deck.InGame.Agent.CoffeeMachine
{
    public class DeckAgentAgentCoffeeMachine : DeckAgentBuilding
    {
        protected override void InternalAfterBuildingInitialized()
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

        public override Vector3[] GetAccessPosition()
        {
            var rotationCount = Mathf.RoundToInt(transform.localRotation.eulerAngles.y / 90f) % 4;
            var rotatedIndices = BuildingData.AccessIndices.GetRotatedIndices(rotationCount);

            var result = new Vector3[rotatedIndices.Length];

            for (var i = 0; i < rotatedIndices.Length; i++)
            {
                result[i] = rotatedIndices[i].ToVector3() + transform.position;
            }

            return result;
        }
    }
}