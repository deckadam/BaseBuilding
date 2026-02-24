using System;
using UnityEngine;

namespace Services.Building.Buildable.Data.Parameter.Implementations
{
    [Serializable]
    public class DeckBuildableParameterBuildOnTop : DeckBuildableParameter
    {
        public override DeckBuildableParameterType ParameterType => DeckBuildableParameterType.BuildableToPlaceOnTop;

        [SerializeField] private DeckBuildable agentToBuildOnTop;
        
        protected override object GetValueInternal()
        {
            return agentToBuildOnTop;
        }

        protected override object GetDefaultValueInternal()
        {
            return agentToBuildOnTop;
        }
    }
}