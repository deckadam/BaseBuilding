using System;

namespace Data.Buildable.Data.Parameter
{
    [Serializable]
    public abstract class DeckBuildableParameter
    {
        public abstract DeckBuildableParameterType ParameterType { get; }
    }
}