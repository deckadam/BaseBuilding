using System;
using System.Collections.Generic;
using System.Linq;
using Deck.Item;
using Deck.Save;
using Deck.UI.InGame;
using Deck.Utility;
using Deck.Utility.Iterators;
using Deck.Utility.Logger;
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

        public override bool ReturnItemVisual(DeckItemVisual itemVisual)
        {
            if (!IsWall(itemVisual)) return false;
            var pos = itemVisual.transform.position.ToVector2Int();
            _wallCheckSet.Remove(pos);

            ConnectWalls(pos, 0);

            return true;
        }

        public override bool RequestItemVisual(DeckId prefabId, Vector2Int cellIndex, out DeckItemVisual itemVisual, bool isInternal = true)
        {
            if (!IsWall(prefabId))
            {
                itemVisual = null;
                return false;
            }

            Debug.LogError("Request item visual");
            _wallCheckSet.Add(cellIndex);
            ConnectWalls(cellIndex, 0);
            itemVisual = null;
            return false;
        }

        private void ConnectWalls(Vector2Int changePosition, int depth)
        {
            var checkList = new bool[4];
            var neighbours = changePosition.GetNeighbours();

            if (depth < 2 && _activeWalls.TryGetValue(changePosition, out var itemVisual))
            {
                var result = ReturnIfHasItemVisual(itemVisual);
                if (!result)
                {
                    DeckLogger.Error("ReturnIfHasItemVisual failed!");
                }

                _activeWalls.Remove(changePosition);
            }

            if (!_wallCheckSet.Contains(changePosition))
            {
                return;
            }

            for (var index = 0; index < neighbours.Length; index++)
            {
                var neighbour = neighbours[index];
                var isWall = _wallCheckSet.Contains(neighbour);

                checkList[index] = isWall || _doorCheckSet.Contains(neighbour);

                if (depth < 1 && isWall)
                {
                    ConnectWalls(neighbour, depth + 1);
                }
            }

            Debug.LogError(changePosition);
            var visualPrefab = GetVisualToPlace(checkList);
            RentIfHasItemVisual(visualPrefab.PrefabId, out var visualInstance, false);
            _activeWalls[changePosition] = visualInstance;
            visualInstance.transform.position = changePosition.ToVector3();
        }


        private DeckItemVisual GetVisualToPlace(IReadOnlyList<bool> checkList)
        {
            var trueCount = 0;
            foreach (var b in checkList)
            {
                if (b)
                {
                    trueCount++;
                }
            }

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
                    return _visualDictionary[WallStyle.TwoCornerUpperLeft];
                }

                if (checkList[0] && checkList[3])
                {
                    return _visualDictionary[WallStyle.TwoCornerLowerLeft];
                }

                if (checkList[1] && checkList[2])
                {
                    return _visualDictionary[WallStyle.TwoCornerUpperRight];
                }

                if (checkList[1] && checkList[3])
                {
                    return _visualDictionary[WallStyle.TwoCornerLowerRight];
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