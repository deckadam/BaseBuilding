using Services.Building.Buildable.Data.Parameter;

namespace Services.Building.Buildable.Data
{
    public class DeckBuildableParameterAccessor<T> where T : DeckBuildableParameter
    {
        public bool HasReference { get; }

        private T _parameterReference;

        public DeckBuildableParameterAccessor(DeckBuildable buildable)
        {
            HasReference = buildable.TryGetParameter(out _parameterReference);
        }

        public J GetValue<J>() where J : class
        {
            return _parameterReference.GetValue<J>();
        }

        public J GetPrimitiveValue<J>() where J : struct
        {
            return _parameterReference.GetPrimitiveValue<J>();
        }
    }
}