using System;
using UI.Building.BuildMode;
using UnityEngine;

namespace Data.Buildable.Data.Parameter.Implementations.Build
{
    [Serializable]
    public class DeckBuildableParameterBuildMode : DeckBuildableParameter
    {
        public override DeckBuildableParameterType ParameterType => DeckBuildableParameterType.BuildMode;

        [SerializeField] private DeckBuildMode buildMode;

        protected override object GetValueInternal()
        {
            return buildMode;
        }

        protected override object GetDefaultValueInternal()
        {
            return buildMode;
        }
    }
}