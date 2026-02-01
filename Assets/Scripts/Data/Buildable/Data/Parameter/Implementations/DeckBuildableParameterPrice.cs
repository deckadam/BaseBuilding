using System;
using Data.Currency;
using UnityEngine;

namespace Data.Buildable.Data.Parameter.Implementations
{
    [Serializable]
    public class DeckBuildableParameterPrice : DeckBuildableParameter
    {
        public override DeckBuildableParameterType ParameterType => DeckBuildableParameterType.Price;

        [SerializeField] private DeckPrice[] prices;

        protected override object GetValueInternal()
        {
            return prices;
        }

        protected override object GetDefaultValueInternal()
        {
            return prices;
        }
    }
}