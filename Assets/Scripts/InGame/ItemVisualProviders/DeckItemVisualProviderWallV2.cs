using System;
using System.Collections.Generic;
using System.Linq;
using Deck.Item;
using Deck.Save;
using Deck.UI.InGame;
using Deck.Utility;
using Deck.Utility.Iterators;
using Unity.VisualScripting;
using UnityEngine;

namespace Deck.ItemVisualProviders
{
    [CreateAssetMenu(fileName = "DeckItemVisualProviderWallV2", menuName = "Service/ItemVisualManager/DeckItemVisualProviderWallV2")]
    public class DeckItemVisualProviderWallV2 : DeckItemVisualProviderBasic
    {
        [SerializeField] private WallVisual[] wallItemVisualPrefabs;

        private Dictionary<WallStyle, DeckItemVisual> _visualDictionary;
        private Dictionary<Vector2Int, DeckItemVisual> _activeWalls;
        private HashSet<Vector2Int> _wallCheckSet;

        protected override void OnInitialize()
        {
            _visualDictionary = new Dictionary<WallStyle, DeckItemVisual>();
            _activeWalls = new Dictionary<Vector2Int, DeckItemVisual>();
            _wallCheckSet = new HashSet<Vector2Int>();

            foreach (var wallVisual in wallItemVisualPrefabs)
            {
                _visualDictionary[wallVisual.wallStyle] = wallVisual.wallVisual;
            }
        }

        protected override void OnSpawned(DeckItemVisual itemVisual)
        {
        }

        protected override void OnDespawned(DeckItemVisual itemVisual)
        {
        }

        public override bool ReturnItemVisual(DeckItemVisual itemVisual)
        {
            if (!IsWall(itemVisual)) return false;
            var pos = itemVisual.transform.position.ToVector2Int();
            _wallCheckSet.Remove(pos);

            ConnectWalls(pos, 0);

            return true;
        }

        public override bool RequestItemVisual(DeckId prefabId, Vector2Int cellIndex, out DeckItemVisual itemVisual, bool alert = true)
        {
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
                _activeWalls.Remove(changePosition);
            }

            for (var index = 0; index < neighbours.Length; index++)
            {
                var neighbour = neighbours[index];

                checkList[index] = _wallCheckSet.Contains(neighbour);

                if (depth < 1 && _wallCheckSet.Contains(neighbour))
                {
                    ConnectWalls(neighbour, depth + 1);
                }
            }

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
                Debug.LogError("Four corner wall detected");
                return _visualDictionary[WallStyle.FourCorner];
            }

            if (trueCount == 3)
            {
                if (!checkList[0])
                {
                    Debug.LogError("Three corner left wall detected");
                    return _visualDictionary[WallStyle.ThreeCornerLeft];
                }

                if (!checkList[1])
                {
                    Debug.LogError("Three corner right wall detected");
                    return _visualDictionary[WallStyle.ThreeCornerRight];
                }

                if (!checkList[2])
                {
                    Debug.LogError("Three corner lower wall detected");
                    return _visualDictionary[WallStyle.ThreeCornerLower];
                }

                if (!checkList[3])
                {
                    Debug.LogError("Three corner upper wall detected");
                    return _visualDictionary[WallStyle.ThreeCornerUpper];
                }
            }

            if (trueCount == 2)
            {
                if (checkList[0] && checkList[2])
                {
                    Debug.LogError("TwoCornerUpperLeft");

                    return _visualDictionary[WallStyle.TwoCornerUpperLeft];
                }

                if (checkList[0] && checkList[3])
                {
                    Debug.LogError("TwoCornerLowerLeft");
                    return _visualDictionary[WallStyle.TwoCornerLowerLeft];
                }

                if (checkList[1] && checkList[2])
                {
                    Debug.LogError("TwoCornerUpperRight");
                    return _visualDictionary[WallStyle.TwoCornerUpperRight];
                }

                if (checkList[1] && checkList[3])
                {
                    Debug.LogError("TwoCornerLowerRight");
                    return _visualDictionary[WallStyle.TwoCornerLowerRight];
                }

                if (checkList[0] && checkList[1])
                {
                    Debug.LogError("Vertical");
                    return _visualDictionary[WallStyle.Vertical];
                }

                if (checkList[2] && checkList[3])
                {
                    Debug.LogError("Horizontal");
                    return _visualDictionary[WallStyle.Horizontal];
                }

                Debug.LogError("Huh");
            }

            if (trueCount == 1)
            {
                if (checkList[0] || checkList[1])
                {
                    Debug.LogError("Vertical");
                    return _visualDictionary[WallStyle.Vertical];
                }

                if (checkList[2] || checkList[3])
                {
                    Debug.LogError("Horizontal");
                    return _visualDictionary[WallStyle.Horizontal];
                }
            }

            Debug.LogError("Empty wall detected");
            return _visualDictionary[WallStyle.Empty];
        }

        private bool IsWall(DeckItemVisual itemVisual)
        {
            return wallItemVisualPrefabs.Any(item => item.wallVisual.PrefabId.Equals(itemVisual.PrefabId));
        }


        protected override string OnSaveDataRequested()
        {
            return null;
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