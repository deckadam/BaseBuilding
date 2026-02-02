using UI.Building.BuildMode;
using UnityEngine;

namespace Services.Building.Buildable.Data.Parameter.Implementations
{
    public class DeckBuildableParameterRotationMode : DeckBuildableParameter
    {
        public override DeckBuildableParameterType ParameterType => DeckBuildableParameterType.RotationMode;

        [SerializeField] private DeckRotationMode rotationMode;

        protected override object GetValueInternal()
        {
            return rotationMode;
        }

        protected override object GetDefaultValueInternal()
        {
            return rotationMode;
        }
    }
}