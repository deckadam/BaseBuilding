using System.Collections.Generic;
using System.Linq;
using Base;
using EventManager;
using ItemVisualProviders.Door.Events;
using Services.Map;
using UnityEngine;
using Utility;
using Utility.Iterators;

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

        protected override void InternalOnInitialize()
        {
            _activeWalls = new Dictionary<Vector2Int, DeckItemVisual>();
            _activeWallAgents = new Dictionary<Vector2Int, DeckAgent>();
            _wallCheckSet = new HashSet<Vector2Int>();
            _doorCheckSet = new HashSet<Vector2Int>();

            DeckEventManager.Register<DeckEventOnDoorPlaced>(OnDoorPlaced);
            DeckEventManager.Register<DeckEventOnDoorRemoved>(OnDoorRemoved);
        }

        protected override void InternalOnDeInitialize()
        {
            DeckEventManager.Unregister<DeckEventOnDoorPlaced>(OnDoorPlaced);
            DeckEventManager.Unregister<DeckEventOnDoorRemoved>(OnDoorRemoved);
        }

        private void OnDoorPlaced(DeckEventOnDoorPlaced obj)
        {
            if (_wallCheckSet.Contains(obj.position))
            {
                TryReturnItemVisual(_activeWalls[obj.position]);
                _activeWalls.Remove(obj.position);
                _activeWallAgents.Remove(obj.position);
                _wallCheckSet.Remove(obj.position);
            }

            _doorCheckSet.Add(obj.position);

            var neighbours = obj.position.GetNeighbours();
            foreach (var neighbour in neighbours)
            {
                if (!_activeWalls.TryGetValue(neighbour, out var temp)) continue;

                TryReturnItemVisual(temp);
                PlaceItemVisual(neighbour);
            }
        }

        private void OnDoorRemoved(DeckEventOnDoorRemoved obj)
        {
            _doorCheckSet.Remove(obj.position);
            var neighbours = obj.position.GetNeighbours();
            foreach (var neighbour in neighbours)
            {
                if (!_wallCheckSet.Contains(neighbour))
                    continue;

                TryReturnItemVisual(_activeWalls[neighbour]);
                PlaceWallWithNeighbours(neighbour);
            }
        }

        public override bool ReturnItemVisual(DeckItemVisual itemVisual)
        {
            if (!IsSupportedItemVisual(itemVisual)) return false;

            TryReturnItemVisual(itemVisual);
            var itemPos = itemVisual.transform.position.ToVector2Int();

            _activeWalls.Remove(itemPos);
            _activeWallAgents.Remove(itemPos);
            _wallCheckSet.Remove(itemPos);

            var neighbours = itemPos.GetNeighbours();
            foreach (var neighbour in neighbours)
            {
                if (!_wallCheckSet.Contains(neighbour))
                    continue;

                TryReturnItemVisual(_activeWalls[neighbour]);
                PlaceWallWithNeighbours(neighbour);
            }

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

        public override bool RequestBulkItemVisuals(DeckAgent[] agents, DeckId prefabId, Vector2Int[] indices, out DeckItemVisual[] itemVisuals)
        {
            if (!IsSupportedItemVisual(prefabId))
            {
                itemVisuals = null;
                return false;
            }

            for (var i = 0; i < indices.Length; i++)
            {
                var index = indices[i];
                _wallCheckSet.Add(index);
                _activeWallAgents[index] = agents[i];
            }

            var checkList = new bool[4];
            itemVisuals = new DeckItemVisual[indices.Length];
            for (var i = 0; i < indices.Length; i++)
            {
                var position = indices[i];
                var neighbours = position.GetNeighbours();
                for (var index = 0; index < neighbours.Length; index++)
                {
                    var neighbour = neighbours[index];
                    var isWall = _wallCheckSet.Contains(neighbour);
                    var isWallOrDoor = isWall || _doorCheckSet.Contains(neighbour);
                    checkList[index] = isWallOrDoor;
                }

                var itemVisualPrefab = GetVisualToPlace(checkList);
                RentIfHasItemVisual(itemVisualPrefab.PrefabId, out var newInstance);

                var agent = _activeWallAgents[position];
                agent.SetItemVisual(newInstance);
                newInstance.SetAgent(agent);

                _activeWalls[position] = newInstance;
                itemVisuals[i] = newInstance;
            }
            return true;
        }

        public void RequestFill(Vector2Int minIndex, Vector2Int maxIndex)
        {
            for (var x = minIndex.x; x < maxIndex.x; x++)
            {
                for (var y = minIndex.y; y < maxIndex.y; y++)
                {
                    _wallCheckSet.Add(new Vector2Int(x, y));
                }
            }

            var mapTransform = DeckServiceScene.GetMap().transform;
            for (var x = minIndex.x; x < maxIndex.x; x++)
            {
                for (var y = minIndex.y; y < maxIndex.y; y++)
                {
                    var itemVisual = PlaceItemVisual(new Vector2Int(x, y), false, false);
                    itemVisual.transform.SetParent(mapTransform);
                    itemVisual.transform.position = new Vector3(x, 0, y);
                }
            }
        }

        private DeckItemVisual PlaceWallWithNeighbours(Vector2Int position)
        {
            var neighbours = position.GetNeighbours();

            foreach (var neighbour in neighbours)
            {
                if (!_wallCheckSet.Contains(neighbour)) continue;
                TryReturnItemVisual(_activeWalls[neighbour]);
                PlaceItemVisual(neighbour);
            }

            return PlaceItemVisual(position);
        }

        private DeckItemVisual PlaceItemVisual(Vector2Int position, bool setItemVisual = true, bool setAgent = true)
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

            var itemVisualPrefab = GetVisualToPlace(checkList);
            RentIfHasItemVisual(itemVisualPrefab.PrefabId, out var itemVisualInstance);

            if (setItemVisual)
            {
                var agent = _activeWallAgents[position];
                agent.SetItemVisual(itemVisualInstance);
            }

            if (setAgent)
            {
                var agent = _activeWallAgents[position];
                itemVisualInstance.SetAgent(agent);
            }

            _activeWalls[position] = itemVisualInstance;

            return itemVisualInstance;
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
            var neighbours = cellIndex.GetNeighbours();
            return neighbours.Count(pos => _wallCheckSet.Contains(pos));
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