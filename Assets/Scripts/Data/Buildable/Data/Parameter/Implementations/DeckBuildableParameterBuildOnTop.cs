using System;

namespace Data.Buildable.Data.Parameter.Implementations
{
    [Serializable]
    public class DeckBuildableParameterBuildOnTop : DeckBuildableParameter
    {
        public override DeckBuildableParameterType ParameterType => DeckBuildableParameterType.BuildableToPlaceOnTop;
    }
}