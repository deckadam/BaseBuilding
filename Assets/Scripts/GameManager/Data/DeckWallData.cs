using System;
using Services.Building.Buildable;
using UnityEngine;

namespace GameManager.Data
{
    [Serializable]
    public class DeckWallData
    {
        [SerializeField] private DeckWallType wallType;
        [SerializeField] private DeckBuildable wallBuildable;

        public DeckWallType WallType => wallType;
        public DeckBuildable WallBuildable => wallBuildable;
    }
}