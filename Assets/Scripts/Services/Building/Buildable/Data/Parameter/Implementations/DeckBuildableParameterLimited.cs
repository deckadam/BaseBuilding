using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Services.Building.Buildable.Data.Parameter.Implementations
{
    [Serializable]
    public class DeckBuildableParameterLimited : DeckBuildableParameter
    {
        public override DeckBuildableParameterType ParameterType => DeckBuildableParameterType.Limited;

        [SerializeField] private int limit;

        [ReadOnly, SerializeField] private int currentLimit;

        protected override object GetValueInternal()
        {
            return currentLimit;
        }

        protected override object GetDefaultValueInternal()
        {
            return limit;
        }

        public void SetCurrentLimit(int newLimit)
        {
            if (_isOverridden)
            {
                var convertedValue = _overrideValue as DeckBuildableParameterLimited;
                convertedValue.currentLimit = newLimit;
            }
            else
            {
                currentLimit = newLimit;
            }
        }

        public int GetMaxLimit()
        {
            if (_isOverridden)
            {
                var convertedValue = _overrideValue as DeckBuildableParameterLimited;
                return convertedValue.limit;
            }
            else
            {
                return limit;
            }
        }
    }
}