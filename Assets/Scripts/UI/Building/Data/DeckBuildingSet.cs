using System;
using Services.Building.Buildable.Data;
using UnityEngine;

namespace UI.Building.Data
{
    [Serializable]
    public struct DeckBuildingSet
    {
        [SerializeField] private DeckBuildableCategory category;

        [SerializeField] private DeckBuildingButton button;
        [SerializeField] private DeckBuildingPage page;

        public DeckBuildableCategory Category => category;
        public DeckBuildingButton Button => button;
        public DeckBuildingPage Page => page;
    }
}