using System;
using Data.Buildable.Data.Parameter.Implementations;

namespace Data.Buildable.Data.Parameter
{
    public static class DeckBuildableParameterTypeResolver
    {
        public static DeckBuildableParameterType ResolveParameterType<T>() where T : DeckBuildableParameter
        {
            return typeof(T) switch
            {
                { } t when t == typeof(DeckBuildableParameterIcon) => DeckBuildableParameterType.Icon,
                { } t when t == typeof(DeckBuildableParameterLimited) => DeckBuildableParameterType.Limited,
                { } t when t == typeof(DeckBuildableParameterPrice) => DeckBuildableParameterType.Limited,
                { } t when t == typeof(DeckBuildableParameterBuildMode) => DeckBuildableParameterType.BuildMode,
                { } t when t == typeof(DeckBuildableParameterBuildOnTop) => DeckBuildableParameterType.BuildableToPlaceOnTop,
                _ => throw new Exception("Unknown deck buildable parameter type")
            };
        }
    }
}