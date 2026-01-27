using System;

namespace Data.Buildable.Data.Parameter.Implementations
{
    [Serializable]
    public class DeckBuildableParameterLimited : DeckBuildableParameter
    {
        public override DeckBuildableParameterType ParameterType => DeckBuildableParameterType.Limited;
    }
}