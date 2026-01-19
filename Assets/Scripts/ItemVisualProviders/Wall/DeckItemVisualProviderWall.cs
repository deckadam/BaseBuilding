using System;
using System.Collections.Generic;
using Base;
using EventManager;
using GameManager;
using GameManager.Events;
using Instancing;
using ItemVisualProviders.Door.Events;
using ItemVisualProviders.Wall.SubProviders;
using UnityEngine;
using Utility;
using Utility.Iterators;

namespace ItemVisualProviders.Wall
{
    [CreateAssetMenu(fileName = "DeckItemVisualProviderWall", menuName = "Service/ItemVisualManager/DeckItemVisualProviderWall")]
    public class DeckItemVisualProviderWall : DeckItemVisualProviderBasic
    {
        [SerializeField] private DeckItemVisualProviderWallBasic wallProviderBasic;
        [SerializeField] private DeckItemVisualProviderWallFourNeighbour wallProviderFourNeighbour;
        [SerializeField] private DeckItemVisualProviderEightNeighbour wallProviderEightNeighbour;

        private Dictionary<Vector2Int, DeckItemVisual> _activeWalls;
        private Dictionary<Vector2Int, DeckAgent> _activeWallAgents;
        private HashSet<Vector2Int> _wallCheckSet;
        private HashSet<Vector2Int> _doorCheckSet;

        private DeckItemVisualProviderWallBasic _activeWallProvider;

        protected override void InternalOnInitialize()
        {
            _activeWalls = new Dictionary<Vector2Int, DeckItemVisual>();
            _activeWallAgents = new Dictionary<Vector2Int, DeckAgent>();
            _wallCheckSet = new HashSet<Vector2Int>();
            _doorCheckSet = new HashSet<Vector2Int>();

            DeckEventManager.Register<DeckEventOnDoorPlaced>(OnDoorPlaced);
            DeckEventManager.Register<DeckEventOnDoorRemoved>(OnDoorRemoved);
            DeckEventManager.Register<DeckEventOnGameSettingsLoaded>(OnGameSettingsLoaded);
        }

        protected override void InternalOnDeInitialize()
        {
            DeckEventManager.Unregister<DeckEventOnDoorPlaced>(OnDoorPlaced);
            DeckEventManager.Unregister<DeckEventOnDoorRemoved>(OnDoorRemoved);
            DeckEventManager.Unregister<DeckEventOnGameSettingsLoaded>(OnGameSettingsLoaded);
        }

        private void OnGameSettingsLoaded(DeckEventOnGameSettingsLoaded obj)
        {
            _activeWallProvider = obj.gameSetting.GetWallType() switch
            {
                DeckWallType.Cube => wallProviderBasic,
                DeckWallType.FourNeighbour => wallProviderFourNeighbour,
                DeckWallType.EightNeighbour => wallProviderEightNeighbour,
                _ => throw new Exception("Unknown wall type")
            };
            
            _activeWallProvider.Initialize(this);
        }

        private void OnDoorPlaced(DeckEventOnDoorPlaced obj)
        {
            if (_wallCheckSet.Contains(obj.position))
            {
                ReturnItemVisualToPool(_activeWalls[obj.position]);
                _activeWalls.Remove(obj.position);
                _activeWallAgents.Remove(obj.position);
                _wallCheckSet.Remove(obj.position);
            }

            _doorCheckSet.Add(obj.position);

            var neighbours = obj.position.GetNeighbours();
            foreach (var neighbour in neighbours)
            {
                if (!_activeWalls.TryGetValue(neighbour, out var temp)) continue;

                ReturnItemVisualToPool(temp);
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

                ReturnItemVisualToPool(_activeWalls[neighbour]);
                PlaceWallWithNeighbours(neighbour);
            }
        }

        public override void ReturnItemVisual(DeckItemVisual itemVisual)
        {
            ReturnItemVisualToPool(itemVisual);
            var itemPos = itemVisual.transform.position.ToVector2Int();

            _activeWalls.Remove(itemPos);
            _activeWallAgents.Remove(itemPos);
            _wallCheckSet.Remove(itemPos);

            var neighbours = itemPos.GetNeighbours();
            foreach (var neighbour in neighbours)
            {
                if (!_wallCheckSet.Contains(neighbour))
                    continue;

                ReturnItemVisualToPool(_activeWalls[neighbour]);
                PlaceWallWithNeighbours(neighbour);
            }
        }

        public override bool RequestItemVisual(DeckAgent agent, DeckId prefabId, Vector2Int cellIndex, out DeckItemVisual itemVisual)
        {
            _activeWallAgents[cellIndex] = agent;
            _wallCheckSet.Add(cellIndex);
            itemVisual = PlaceWallWithNeighbours(cellIndex);
            return true;
        }

        public override bool RequestBulkItemVisuals(DeckAgent[] agents, DeckId prefabId, Vector2Int[] indices, out DeckItemVisual[] itemVisuals)
        {
            for (var i = 0; i < indices.Length; i++)
            {
                var index = indices[i];
                _wallCheckSet.Add(index);
                _activeWallAgents[index] = agents[i];
            }

            var checkList = new bool[_activeWallProvider.NeighbourSetSize];
            itemVisuals = new DeckItemVisual[indices.Length];
            for (var i = 0; i < indices.Length; i++)
            {
                var position = indices[i];
                var itemVisualPrefab = _activeWallProvider.GetVisualToPlace(position, ref checkList);
                RentIfHasItemVisual(itemVisualPrefab.PrefabId, out var newInstance);

                var agent = _activeWallAgents[position];
                agent.SetItemVisual(newInstance);
                newInstance.SetAgent(agent);

                _activeWalls[position] = newInstance;
                itemVisuals[i] = newInstance;
            }

            return true;
        }

        private DeckItemVisual PlaceWallWithNeighbours(Vector2Int position)
        {
            var neighbours = position.GetNeighbours();

            foreach (var neighbour in neighbours)
            {
                if (!_wallCheckSet.Contains(neighbour)) continue;
                ReturnItemVisualToPool(_activeWalls[neighbour]);
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

            var itemVisualPrefab = _activeWallProvider.GetVisualToPlace(position, ref checkList);
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

        public bool HasWallOnPosition(Vector2Int cellIndex)
        {
            return _wallCheckSet.Contains(cellIndex);
        }

        public bool IsWall(Vector2Int neighbour)
        {
            return _wallCheckSet.Contains(neighbour);
        }

        public bool IsDoor(Vector2Int neighbour)
        {
            return _doorCheckSet.Contains(neighbour);
        }
    }
}