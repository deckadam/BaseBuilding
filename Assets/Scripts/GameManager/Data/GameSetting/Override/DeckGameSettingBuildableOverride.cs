using System;
using Data.Buildable;
using Data.Currency;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameManager.Data.GameSetting.Override
{
    [Serializable]
    public struct DeckGameSettingBuildableOverride
    {
        [SerializeField] private DeckBuildable buildable;

        [SerializeField] private bool overrideLimit;
        [SerializeField, ShowIf(nameof(overrideLimit))] private int overridenLimit;

        [SerializeField] private bool overridePrice;
        [SerializeField, ShowIf(nameof(overridePrice))] private DeckPrice[] overridenPrice;

        public DeckBuildable Buildable => buildable;
        
        public bool OverrideLimit => overrideLimit;
        public int OverridenLimit => overridenLimit;

        public bool OverridePrice => overridePrice;
        public DeckPrice[] OverridenPrice => overridenPrice;
    }
}