using System;

namespace Services.Building.Buildable.Data.Parameter
{
    [Serializable]
    public abstract class DeckBuildableParameter
    {
        public abstract DeckBuildableParameterType ParameterType { get; }
        protected abstract object GetValueInternal();
        protected abstract object GetDefaultValueInternal();
        protected DeckBuildableParameter _overrideValue;
        protected bool _isOverridden;

        public virtual void ApplyOverride(DeckBuildableParameter overrideValue)
        {
            _isOverridden = true;
            _overrideValue = overrideValue;
        }

        public void ResetOverride()
        {
            _isOverridden = false;
        }

        public object GetValue()
        {
            return GetValueInternal();
        }

        public T GetValue<T>() where T : class
        {
            return _isOverridden ? _overrideValue.GetValueInternal() as T : GetValueInternal() as T;
        }

        public T GetPrimitiveValue<T>() where T : struct
        {
            var value = _isOverridden ? _overrideValue.GetValueInternal() : GetValueInternal();
            return (T)Convert.ChangeType(value, typeof(T));
        }

        public bool IsOverridden()
        {
            return _isOverridden;
        }
    }
}