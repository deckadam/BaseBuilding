using System;
using System.Collections.Generic;
using Services.Building.Buildable;
using Services.Building.Buildable.Data.Parameter;
using UnityEngine;

namespace GameManager.Data.GameSetting.Override
{
    [Serializable]
    public struct DeckGameSettingBuildableOverride
    {
        [SerializeField] private DeckBuildable buildable;
        [SerializeReference] private List<DeckBuildableParameter> parameters;

        public DeckBuildable Buildable => buildable;
        public List<DeckBuildableParameter> Parameters => parameters;
    }
}