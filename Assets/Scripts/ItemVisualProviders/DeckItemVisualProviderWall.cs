using System.Collections.Generic;
using System.Linq;
using Base;
using Deck.Base;
using Deck.Utility.Iterators;
using UnityEngine;
using Utility;

namespace ItemVisualProviders
{
    [CreateAssetMenu(fileName = "DeckItemVisualProviderWall", menuName = "Service/ItemVisualManager/DeckItemVisualProviderWall")]
    public class DeckItemVisualProviderWall : DeckItemVisualProviderBasic
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

        private Dictionary<Vector2Int, DeckItemVisual> _activeWalls;
        private Dictionary<Vector2Int, DeckAgent> _activeWallAgents;
        private HashSet<Vector2Int> _wallCheckSet;
        private HashSet<Vector2Int> _doorCheckSet;

        private HashSet<int> _supportedPrefabIds;

        protected override void InternalOnInitialize()
        {
            _activeWalls = new Dictionary<Vector2Int, DeckItemVisual>();
            _activeWallAgents = new Dictionary<Vector2Int, DeckAgent>();
            _wallCheckSet = new HashSet<Vector2Int>();
            _doorCheckSet = new HashSet<Vector2Int>();
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
            if (!IsSupportedItemVisual(itemVisual)) return false;

            ReturnIfHasItemVisual(itemVisual);
            var itemPos = itemVisual.transform.position.ToVector2Int();

            _activeWalls.Remove(itemPos);
            _activeWallAgents.Remove(itemPos);
            _wallCheckSet.Remove(itemPos);

            return true;
        }

        public override bool RequestItemVisual(DeckAgent agent, DeckId prefabId, Vector2Int cellIndex, out DeckItemVisual itemVisual)
        {
            if (!IsSupportedItemVisual(prefabId))
            {
                itemVisual = null;
                return false;
            }

            _activeWallAgents[cellIndex] = agent;
            _wallCheckSet.Add(cellIndex);
            itemVisual = PlaceWallWithNeighbours(cellIndex);
            return true;
        }

        public override bool RequestMultipleItemVisuals(DeckAgent agent, DeckId prefabId, Vector2Int[] indices, out DeckItemVisual[] itemVisuals)
        {
            if (!IsSupportedItemVisual(prefabId))
            {
                itemVisuals = null;
                return false;
            }
            
            foreach (var index in indices)
            {
                _wallCheckSet.Add(index);
                _activeWallAgents[index] = agent;
            }

            itemVisuals = new DeckItemVisual[indices.Length];
            for (var i = 0; i < indices.Length; i++)
            {
                var index = indices[i];
                itemVisuals[i] = PlaceItemVisual(index, false);
            }

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

        private DeckItemVisual PlaceItemVisual(Vector2Int position, bool setItemVisual = true)
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

            if (setItemVisual)
            {
                agent.SetItemVisual(itemVisualInstance);
            }

            itemVisualInstance.SetAgent(agent);

            _activeWalls[position] = itemVisualInstance;

            return itemVisualInstance;
        }

        private DeckItemVisual GetVisualToPlace(IReadOnlyList<bool> checkList)
        {
            var trueCount = checkList.Count(b => b);

            if (trueCount == 4)
            {
                return fourCornerPrefab;
            }

            if (trueCount == 3)
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
            }

            if (trueCount == 2)
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
            }

            if (trueCount == 1)
            {
                if (checkList[0] || checkList[1])
                {
                    return horizontalPrefab;
                }

                if (checkList[2] || checkList[3])
                {
                    return verticalPrefab;
                }
            }

            return emptyPrefab;
        }

        public (Vector2Int, bool)[] GetNeighbourWallSet(Vector2Int cellIndex)
        {
            var result = new (Vector2Int, bool)[8];
            var neighbours = cellIndex.GetRectNeighbours();
            for (var index = 0; index < neighbours.Length; index++)
            {
                var pos = neighbours[index];
                if (_wallCheckSet.Contains(pos))
                {
                    result[index] = (pos, true);
                }
            }

            return result;
        }


        public int GetNeighbourCount(Vector2Int cellIndex)
        {
            var count = 0;
            var neighbours = cellIndex.GetNeighbours();
            for (var index = 0; index < neighbours.Length; index++)
            {
                var pos = neighbours[index];
                if (_wallCheckSet.Contains(pos))
                {
                    count++;
                }
            }

            return count;
        }

        public bool[] GetNeighbourSetRect(Vector2Int cellIndex)
        {
            var result = new bool[4];
            var neighbours = cellIndex.GetRectNeighbours();
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

        public bool HasWallOnPosition(Vector2Int cellIndex)
        {
            return _wallCheckSet.Contains(cellIndex);
        }

        public bool IsWall(Vector2Int neighbour)
        {
            return _wallCheckSet.Contains(neighbour);
        }
    }
}