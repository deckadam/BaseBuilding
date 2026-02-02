using System;
using UI.Building.BuildMode;
using UnityEngine;

namespace Services.Building.Buildable.Data.Parameter.Implementations.Build
{
    public class DeckBuildableParameterBuildModeGridBased : DeckBuildableParameter
    {
        public override DeckBuildableParameterType ParameterType => DeckBuildableParameterType.BuildMode;

        [SerializeField] private DeckGridBasedData gridBasedData;

        protected override object GetValueInternal()
        {
            return gridBasedData;
        }

        protected override object GetDefaultValueInternal()
        {
            return gridBasedData;
        }

        public DeckBuildableParameterBuildModeGridBased(DeckGridBasedData deckGridBasedData)
        {
            gridBasedData = deckGridBasedData;
        }
    }

    [Serializable]
    public class DeckGridBasedData
    {
        [SerializeField] private DeckBuildMode buildMode;
        [SerializeField] private Vector2Int[] indices;
        [SerializeField] private Vector2Int[] accessIndices;

        public DeckBuildMode BuildMode => buildMode;
        public Vector2Int[] Indices => indices;
        public Vector2Int[] AccessIndices => accessIndices;

        public DeckGridBasedData(DeckBuildMode buildMode, Vector2Int[] indices, Vector2Int[] accessIndices)
        {
            this.buildMode = buildMode;
            this.indices = indices;
            this.accessIndices = accessIndices;
        }
    }
}