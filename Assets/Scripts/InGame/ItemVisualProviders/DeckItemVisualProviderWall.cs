using System;
using System.Collections.Generic;
using System.Linq;
using Deck.Base.Id;
using Deck.Components;
using Deck.Utility;
using Deck.Utility.Iterators;
using Deck.Utility.Logger;
using UnityEngine;

namespace Deck.ItemVisualProviders
{
    [CreateAssetMenu(fileName = "DeckItemVisualProviderWall", menuName = "Service/ItemVisualManager/DeckItemVisualProviderWall")]
    public class DeckItemVisualProviderWall : DeckItemVisualProviderBasic
    {
        [SerializeField] private WallVisual[] wallItemVisualPrefabs;

        private Dictionary<WallStyle, DeckItemVisual> _visualDictionary;
        private Dictionary<Vector2Int, DeckItemVisual> _activeWalls;
        private Dictionary<Vector2Int, DeckAgent> _activeWallAgents;
        private HashSet<Vector2Int> _wallCheckSet;
        private HashSet<Vector2Int> _doorCheckSet;

        protected override void OnInitialize()
        {
            _visualDictionary = new Dictionary<WallStyle, DeckItemVisual>();
            _activeWalls = new Dictionary<Vector2Int, DeckItemVisual>();
            _activeWallAgents = new Dictionary<Vector2Int, DeckAgent>();
            _wallCheckSet = new HashSet<Vector2Int>();
            _doorCheckSet = new HashSet<Vector2Int>();

            foreach (var wallVisual in wallItemVisualPrefabs)
            {
                _visualDictionary[wallVisual.wallStyle] = wallVisual.wallVisual;
            }
        }

        public void OnDoorPlaced(Vector2Int cellIndex)
        {
            if (_wallCheckSet.Contains(cellIndex))
            {
                ReturnIfHasItemVisual(_activeWalls[cellIndex]);
                _activeWalls.Remove(cellIndex);
                _activeWallAgents.Remove(cellIndex);
                _wallCheckSet.Remove(cellIndex);
            }

            _doorCheckSet.Add(cellIndex);

            var neighbours = cellIndex.GetNeighbours();
            foreach (var neighbour in neighbours)
            {
                if (!_activeWalls.TryGetValue(neighbour, out var temp)) continue;

                ReturnIfHasItemVisual(temp);
                PlaceItemVisual(neighbour);
            }
        }

        public override bool ReturnItemVisual(DeckItemVisual itemVisual)
        {
            if (!IsWall(itemVisual)) return false;

            ReturnIfHasItemVisual(itemVisual);
            var itemPos = itemVisual.transform.position.ToVector2Int();

            _activeWalls.Remove(itemPos);
            _activeWallAgents.Remove(itemPos);
            _wallCheckSet.Remove(itemPos);

            return true;
        }

        public override bool RequestItemVisual(DeckAgent agent, DeckId prefabId, Vector2Int cellIndex, out DeckItemVisual itemVisual)
        {
            if (!IsWall(prefabId))
            {
                itemVisual = null;
                return false;
            }

            _activeWallAgents[cellIndex] = agent;
            _wallCheckSet.Add(cellIndex);
            itemVisual = PlaceWallWithNeighbours(cellIndex);
            return true;
        }

        private DeckItemVisual PlaceWallWithNeighbours(Vector2Int position)
        {
            var neighbours = position.GetNeighbours();

            foreach (var neighbour in neighbours)
            {
                if (!_wallCheckSet.Contains(neighbour)) continue;
                ReturnIfHasItemVisual(_activeWalls[neighbour]);
                PlaceItemVisual(neighbour);
            }

            return PlaceItemVisual(position);
        }

        private DeckItemVisual PlaceItemVisual(Vector2Int position)
        {
            var checkList = new bool[4];

            var neighbours = position.GetNeighbours();
            for (var index = 0; index < neighbours.Length; index++)
            {
                var neighbour = neighbours[index];
                var isWall = _wallCheckSet.Contains(neighbour);
                var isWallOrDoor = isWall || _doorCheckSet.Contains(neighbour);
                checkList[index] = isWallOrDoor;
            }

            var agent = _activeWallAgents[position];
            var itemVisualPrefab = GetVisualToPlace(checkList);
            RentIfHasItemVisual(itemVisualPrefab.PrefabId, out var itemVisualInstance);
            agent.SetItemVisual(itemVisualInstance);
            itemVisualInstance.SetAgent(agent);

            _activeWalls[position] = itemVisualInstance;

            return itemVisualInstance;
        }

        private DeckItemVisual GetVisualToPlace(IReadOnlyList<bool> checkList)
        {
            var trueCount = checkList.Count(b => b);

            if (trueCount == 4)
            {
                return _visualDictionary[WallStyle.FourCorner];
            }

            if (trueCount == 3)
            {
                if (!checkList[0])
                {
                    return _visualDictionary[WallStyle.ThreeCornerLeft];
                }

                if (!checkList[1])
                {
                    return _visualDictionary[WallStyle.ThreeCornerRight];
                }

                if (!checkList[2])
                {
                    return _visualDictionary[WallStyle.ThreeCornerLower];
                }

                if (!checkList[3])
                {
                    return _visualDictionary[WallStyle.ThreeCornerUpper];
                }

                DeckLogger.Error("Huh!!!!");
            }

            if (trueCount == 2)
            {
                if (checkList[0] && checkList[2])
                {
                    return _visualDictionary[WallStyle.TwoCornerUpperRight];
                }

                if (checkList[0] && checkList[3])
                {
                    return _visualDictionary[WallStyle.TwoCornerLowerRight];
                }

                if (checkList[1] && checkList[2])
                {
                    return _visualDictionary[WallStyle.TwoCornerUpperLeft];
                }

                if (checkList[1] && checkList[3])
                {
                    return _visualDictionary[WallStyle.TwoCornerLowerLeft];
                }

                if (checkList[0] && checkList[1])
                {
                    return _visualDictionary[WallStyle.Vertical];
                }

                if (checkList[2] && checkList[3])
                {
                    return _visualDictionary[WallStyle.Horizontal];
                }

                DeckLogger.Error("Huh!!!!");
            }

            if (trueCount == 1)
            {
                if (checkList[0] || checkList[1])
                {
                    return _visualDictionary[WallStyle.Vertical];
                }

                if (checkList[2] || checkList[3])
                {
                    return _visualDictionary[WallStyle.Horizontal];
                }
            }

            return _visualDictionary[WallStyle.Empty];
        }

        private bool IsWall(DeckId prefabId)
        {
            return wallItemVisualPrefabs.Any(item => item.wallVisual.PrefabId.Equals(prefabId));
        }

        private bool IsWall(DeckItemVisual itemVisual)
        {
            return wallItemVisualPrefabs.Any(item => item.wallVisual.PrefabId.Equals(itemVisual.PrefabId));
        }

        public bool[] GetNeighbourSet(Vector2Int cellIndex)
        {
            var result = new bool[4];
            var neighbours = cellIndex.GetNeighbours();
            for (var index = 0; index < neighbours.Length; index++)
            {
                var pos = neighbours[index];
                if (_wallCheckSet.Contains(pos))
                {
                    result[index] = true;
                }
            }

            return result;
        }

        public void OnDoorRemoved(Vector2Int cellIndex)
        {
            _doorCheckSet.Remove(cellIndex);

            var neighbours = cellIndex.GetNeighbours();
            foreach (var neighbour in neighbours)
            {
                if (!_wallCheckSet.Contains(neighbour))
                    continue;
                
                ReturnIfHasItemVisual(_activeWalls[neighbour]);
                PlaceWallWithNeighbours(neighbour);
            }
        }

        [Serializable]
        private struct SaveData
        {
            public Vector2Int[] wallPositions;
        }

        [Serializable]
        private struct WallVisual
        {
            public WallStyle wallStyle;
            public DeckItemVisual wallVisual;
        }

        [Serializable]
        private enum WallStyle
        {
            Horizontal,
            Vertical,

            TwoCornerUpperLeft,
            TwoCornerUpperRight,
            TwoCornerLowerLeft,
            TwoCornerLowerRight,

            ThreeCornerUpper,
            ThreeCornerLower,
            ThreeCornerLeft,
            ThreeCornerRight,

            FourCorner,

            Empty
        }
    }
}