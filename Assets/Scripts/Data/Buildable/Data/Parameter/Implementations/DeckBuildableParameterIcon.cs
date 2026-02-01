using System;
using UnityEngine;

namespace Data.Buildable.Data.Parameter.Implementations
{
    [Serializable]
    public class DeckBuildableParameterIcon : DeckBuildableParameter
    {
        public override DeckBuildableParameterType ParameterType => DeckBuildableParameterType.Icon;

        [SerializeField] private Sprite icon;

        protected override object GetValueInternal()
        {
            return icon;
        }

        protected override object GetDefaultValueInternal()
        {
            return icon;
        }
    }
}