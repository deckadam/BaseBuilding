using System;
using System.Collections.Generic;
using Data.Buildable;
using Data.Buildable.Data.Parameter;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameManager.Data.GameSetting.Override
{
    [Serializable]
    public struct DeckGameSettingBuildableOverride
    {
        [SerializeField] private DeckBuildable buildable;
        [SerializeReference, TypeFilter("@Utility.Resource.DeckResourceLocator.GetAllParameterTypes()")] private List<DeckBuildableParameter> parameters;

        public DeckBuildable Buildable => buildable;
        public List<DeckBuildableParameter> Parameters => parameters;
    }
}