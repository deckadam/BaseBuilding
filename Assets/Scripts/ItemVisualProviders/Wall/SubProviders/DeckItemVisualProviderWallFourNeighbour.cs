using System;
using System.Linq;
using Base;
using UnityEngine;
using Utility;
using Utility.Iterators;

namespace ItemVisualProviders.Wall.SubProviders
{
    [Serializable]
    public class DeckItemVisualProviderWallFourNeighbour : DeckItemVisualProviderWallBasic
    {
        [SerializeField] private DeckItemVisual horizontalPrefab;
        [SerializeField] private DeckItemVisual verticalPrefab;

        [SerializeField] private DeckItemVisual twoCornerUpperLeftPrefab;
        [SerializeField] private DeckItemVisual twoCornerUpperRightPrefab;
        [SerializeField] private DeckItemVisual twoCornerLowerLeftPrefab;
        [SerializeField] private DeckItemVisual twoCornerLowerRightPrefab;

        [SerializeField] private DeckItemVisual threeCornerUpperPrefab;
        [SerializeField] private DeckItemVisual threeCornerLowerPrefab;
        [SerializeField] private DeckItemVisual threeCornerLeftPrefab;
        [SerializeField] private DeckItemVisual threeCornerRightPrefab;

        [SerializeField] private DeckItemVisual fourCornerPrefab;

        [SerializeField] private DeckItemVisual emptyPrefab;

        public override int NeighbourSetSize => 4;

        public override DeckItemVisual GetVisualToPlace(Vector2Int position, ref bool[] checkList)
        {
            var neighbours = position.GetNeighbours();
            for (var index = 0; index < neighbours.Length; index++)
            {
                var neighbour = neighbours[index];
                var isWallOrDoor = _wallProvider.IsWall(neighbour) || _wallProvider.IsDoor(neighbour);
                checkList[index] = isWallOrDoor;
            }

            var trueCount = checkList.Count(b => b);
            switch (trueCount)
            {
                case 4:
                    return fourCornerPrefab;
                case 3:
                {
                    if (!checkList[0])
                    {
                        return threeCornerLeftPrefab;
                    }

                    if (!checkList[1])
                    {
                        return threeCornerRightPrefab;
                    }

                    if (!checkList[2])
                    {
                        return threeCornerLowerPrefab;
                    }

                    if (!checkList[3])
                    {
                        return threeCornerUpperPrefab;
                    }

                    DeckLogger.Error("Huh!!!!");
                    break;
                }
                case 2:
                {
                    if (checkList[0] && checkList[2])
                    {
                        return twoCornerUpperRightPrefab;
                    }

                    if (checkList[0] && checkList[3])
                    {
                        return twoCornerLowerRightPrefab;
                    }

                    if (checkList[1] && checkList[2])
                    {
                        return twoCornerUpperLeftPrefab;
                    }

                    if (checkList[1] && checkList[3])
                    {
                        return twoCornerLowerLeftPrefab;
                    }

                    if (checkList[0] && checkList[1])
                    {
                        return horizontalPrefab;
                    }

                    if (checkList[2] && checkList[3])
                    {
                        return verticalPrefab;
                    }

                    DeckLogger.Error("Huh!!!!");
                    break;
                }
                case 1:
                {
                    if (checkList[0] || checkList[1])
                    {
                        return horizontalPrefab;
                    }

                    if (checkList[2] || checkList[3])
                    {
                        return verticalPrefab;
                    }

                    break;
                }
            }

            return emptyPrefab;
        }
    }
}