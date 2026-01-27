using System;
using Data.Buildable.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UI.Building.Data
{
    [Serializable]
    public struct DeckBuildingSet
    {
        [AssetSelector(Paths = "Assets/Resources/Data/Buildables/Buildable Categories")] [SerializeField] private DeckBuildableCategory category;

        [SerializeField] private DeckBuildingButton button;
        [SerializeField] private DeckBuildingPage page;

        public DeckBuildableCategory Category => category;
        public DeckBuildingButton Button => button;
        public DeckBuildingPage Page => page;
    }
}