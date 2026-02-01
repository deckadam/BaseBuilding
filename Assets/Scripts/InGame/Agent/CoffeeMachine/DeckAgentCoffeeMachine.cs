using Data.Buildable.Data.Parameter.Implementations.Build;
using InGame.Agent.Building;
using Services;
using Services.Building;
using UnityEngine;
using Utility;

namespace InGame.Agent.CoffeeMachine
{
    public class DeckAgentCoffeeMachine : DeckAgentBuilding
    {
        private DeckGridBasedData _data;

        protected override void InternalAfterBuildingInitialized()
        {
            var rotationCount = Mathf.RoundToInt(transform.localRotation.eulerAngles.y / 90f) % 4;
            _data = BuildingData.GetParameter<DeckBuildableParameterBuildModeGridBased>().GetValue<DeckGridBasedData>();
            var rotatedIndices = _data.AccessIndices.GetRotatedIndices(rotationCount);
            DeckServiceProvider.GetService<DeckServiceBuilding>().AddAccessCellReference(transform.position.ToVector2Int(), rotatedIndices);
        }

        protected override void OnAgentDestroyed()
        {
            var cellIndex = transform.position.ToVector2Int();
            var rotationCount = Mathf.RoundToInt(transform.localRotation.eulerAngles.y / 90f) % 4;

            var accessIndices = _data.AccessIndices.GetRotatedIndices(rotationCount);
            DeckServiceProvider.GetService<DeckServiceBuilding>().RemoveAccessCellReference(cellIndex, accessIndices);
        }

        public override Vector3[] GetAccessPosition()
        {
            var rotationCount = Mathf.RoundToInt(transform.localRotation.eulerAngles.y / 90f) % 4;
            var rotatedIndices = _data.AccessIndices.GetRotatedIndices(rotationCount);

            var result = new Vector3[rotatedIndices.Length];

            for (var i = 0; i < rotatedIndices.Length; i++)
            {
                result[i] = rotatedIndices[i].ToVector3() + transform.position;
            }

            return result;
        }
    }
}