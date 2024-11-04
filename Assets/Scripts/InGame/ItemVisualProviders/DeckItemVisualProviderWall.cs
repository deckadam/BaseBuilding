using System;
using System.Collections.Generic;
using System.Linq;
using Deck.Components;
using Deck.Components.Building.InGame;
using Deck.Save;
using Deck.Utility;
using Deck.Utility.Iterators;
using Deck.Utility.Logger;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

namespace Deck.ItemVisualProviders
{
    [CreateAssetMenu(fileName = "DeckItemVisualProviderWall", menuName = "Service/ItemVisualManager/DeckItemVisualProviderWall")]
    public class DeckItemVisualProviderWall : DeckItemVisualProviderBasic
    {
        [SerializeField] private WallVisual[] wallItemVisualPrefabs;

        private Dictionary<WallStyle, DeckItemVisual> _visualDictionary;
        private Dictionary<Vector2Int, DeckItemVisual> _activeWalls;
        private HashSet<Vector2Int> _wallCheckSet;
        private HashSet<Vector2Int> _doorCheckSet;

        protected override void OnInitialize()
        {
            _visualDictionary = new Dictionary<WallStyle, DeckItemVisual>();
            _activeWalls = new Dictionary<Vector2Int, DeckItemVisual>();
            _wallCheckSet = new HashSet<Vector2Int>();
            _doorCheckSet = new HashSet<Vector2Int>();

            foreach (var wallVisual in wallItemVisualPrefabs)
            {
                _visualDictionary[wallVisual.wallStyle] = wallVisual.wallVisual;
            }
        }

        public void ReturnItemVisual(Vector2Int cellIndex, bool isInternal)
        {
            if (!IsWall(cellIndex))
            {
                return;
            }

            _wallCheckSet.Remove(cellIndex);
            ReturnItemVisual(_activeWalls[cellIndex], isInternal);
        }

        public override bool ReturnItemVisual(DeckItemVisual itemVisual, bool isInternal)
        {
            if (!IsWall(itemVisual)) return false;

            var itemPos = itemVisual.transform.position.ToVector2Int();

            ReturnIfHasItemVisual(itemVisual, isInternal);
            _wallCheckSet.Remove(itemPos);
            ConnectWalls(itemPos, 0);

            return true;
        }

        public override bool RequestItemVisual(DeckId prefabId, Vector2Int cellIndex, out DeckItemVisual itemVisual, bool isInternal)
        {
            if (_wallCheckSet.Contains(cellIndex))
            {
                itemVisual = null;
                return true;
            }

            if (!IsWall(prefabId))
            {
                itemVisual = null;
                return false;
            }

            _wallCheckSet.Add(cellIndex);
            ConnectWalls(cellIndex, 0);
            itemVisual = _activeWalls[cellIndex];
            return true;
        }

        private void ConnectWalls(Vector2Int changePosition, int depth)
        {
            var checkList = new bool[4];
            var neighbours = changePosition.GetNeighbours();
            DeckAgent agent = null;

            if (depth < 2 && _activeWalls.TryGetValue(changePosition, out var itemVisual))
            {
                agent = itemVisual.Agent;

                if (!ReturnIfHasItemVisual(itemVisual, true))
                {
                    DeckLogger.Error("ReturnIfHasItemVisual failed!");
                }

                _activeWalls.Remove(changePosition);
            }

            for (var index = 0; index < neighbours.Length; index++)
            {
                var neighbour = neighbours[index];
                var isWall = _wallCheckSet.Contains(neighbour);
                checkList[index] = isWall;

                if (depth < 1 && isWall)
                {
                    ConnectWalls(neighbour, depth + 1);
                }
            }

            if (!_wallCheckSet.Contains(changePosition))
            {
                return;
            }

            var visualPrefab = GetVisualToPlace(checkList);

            RentIfHasItemVisual(visualPrefab.PrefabId, out var visualInstance, true);
            _activeWalls[changePosition] = visualInstance;
            visualInstance.transform.position = changePosition.ToVector3();

            if (!agent)
            {
                return;
            }

            visualInstance.transform.parent = agent.transform;
            visualInstance.Agent = agent;
        }

        [Button]
        public void DebugWalls()
        {
            foreach (var wallPos in _wallCheckSet)
            {
                Debug.LogError(wallPos);
            }
        }

        [Button]
        public void DebugNeighbours(int x, int y)
        {
            var neighbours = new Vector2Int(x, y).GetNeighbours();
            var checkList = new bool[4];
            for (var index = 0; index < neighbours.Length; index++)
            {
                var neighbour = neighbours[index];
                var isWall = _wallCheckSet.Contains(neighbour);

                checkList[index] = isWall || _doorCheckSet.Contains(neighbour);
                Debug.LogError(neighbours[index]);
                Debug.LogError(checkList[index]);
            }

            var visualPrefab = GetVisualToPlace(checkList);
            Debug.LogError(visualPrefab.name);
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

        public bool IsWall(Vector2Int cellIndex)
        {
            return _wallCheckSet.Contains(cellIndex);
        }

        private bool IsWall(DeckItemVisual itemVisual)
        {
            return wallItemVisualPrefabs.Any(item => item.wallVisual.PrefabId.Equals(itemVisual.PrefabId));
        }

        protected override string OnSaveDataRequested()
        {
            var saveData = new SaveData
            {
                wallPositions = _activeWalls.Keys.ToArray()
            };
            return DeckSaveUtility.GetSerializedData(saveData);
        }

        protected override void OnLoadDataRequested(string value)
        {
            var saveData = DeckSaveUtility.GetDeserializedData<SaveData>(value);
            _wallCheckSet.AddRange(saveData.wallPositions);
            foreach (var wallPosition in saveData.wallPositions)
            {
                ConnectWalls(wallPosition, 0);
            }
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

        public void OnDoorPlaced(Vector2Int cellIndex)
        {
            _doorCheckSet.Add(cellIndex);

            var neighbours = cellIndex.GetNeighbours();
            foreach (var neighbour in neighbours)
            {
                ConnectWalls(neighbour, 1);
            }
        }

        public void OnDoorRemoved(Vector2Int cellIndex)
        {
            _doorCheckSet.Remove(cellIndex);

            var neighbours = cellIndex.GetNeighbours();
            foreach (var neighbour in neighbours)
            {
                ConnectWalls(neighbour, 1);
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