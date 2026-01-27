using System;

namespace Data.Buildable.Data.Parameter.Implementations
{
    [Serializable]
    public class DeckBuildableParameterBuildMode : DeckBuildableParameter
    {
        public override DeckBuildableParameterType ParameterType => DeckBuildableParameterType.BuildMode;
    }
}