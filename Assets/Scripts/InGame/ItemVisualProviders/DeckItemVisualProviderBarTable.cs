using System;
using System.Collections.Generic;
using System.Linq;
using Deck.Base.Id;
using Deck.Components;
using Deck.EventManager;
using Deck.Services.Implementations.AreaController.Events;
using Deck.Utility;
using Deck.Utility.Iterators;
using Sirenix.Utilities;
using UnityEngine;
using Zenject;

namespace Deck.ItemVisualProviders
{
    [CreateAssetMenu(fileName = "DeckItemVisualProviderBarTable", menuName = "Service/ItemVisualManager/DeckItemVisualProviderBarTable")]
    public class DeckItemVisualProviderBarTable : DeckItemVisualProviderBasic
    {
        [SerializeField] private DeckItemVisual emptyPrefab;

        [SerializeField] private DeckItemVisual horizontalPrefab;
        [SerializeField] private DeckItemVisual verticalPrefab;

        [SerializeField] private DeckItemVisual twoCornerUpperLeftPrefab;
        [SerializeField] private DeckItemVisual twoCornerUpperRightPrefab;
        [SerializeField] private DeckItemVisual twoCornerLowerLeftPrefab;
        [SerializeField] private DeckItemVisual twoCornerLowerRightPrefab;

        [SerializeField] private DeckItemVisual threeCornerUpPrefab;
        [SerializeField] private DeckItemVisual threeCornerDownPrefab;
        [SerializeField] private DeckItemVisual threeCornerLeftPrefab;
        [SerializeField] private DeckItemVisual threeCornerRightPrefab;

        [SerializeField] private DeckItemVisual fourCornerPrefab;

        [SerializeField] private DeckItemVisual rightWallConnectionPrefab;
        [SerializeField] private DeckItemVisual leftWallConnectionPrefab;
        [SerializeField] private DeckItemVisual upWallConnectionPrefab;
        [SerializeField] private DeckItemVisual downWallConnectionPrefab;

        [SerializeField] private DeckItemVisual upperRightWallConnectionPrefab;
        [SerializeField] private DeckItemVisual upperLeftWallConnectionPrefab;
        [SerializeField] private DeckItemVisual lowerRightWallConnectionPrefab;
        [SerializeField] private DeckItemVisual lowerLeftWallConnectionPrefab;

        [SerializeField] private DeckItemVisual fourCornerWallConnectionUpperLeftSinglePrefab;
        [SerializeField] private DeckItemVisual fourCornerWallConnectionUpperRightSinglePrefab;
        [SerializeField] private DeckItemVisual fourCornerWallConnectionLowerLeftSinglePrefab;
        [SerializeField] private DeckItemVisual fourCornerWallConnectionLowerRightSinglePrefab;


        private Dictionary<Vector2Int, DeckItemVisual> _activeBarTables;
        private Dictionary<Vector2Int, DeckAgent> _activeBarTableAgents;
        private Dictionary<Vector2Int, Dictionary<Vector2Int, DeckItemVisual>> _activeWallConnections;
        private HashSet<Vector2Int> _barTableCheckSet;

        private DeckItemVisualProviderWall _itemVisualProviderWall;

        [Inject]
        private void Inject(DeckItemVisualProviderWall itemVisualProviderWall)
        {
            _itemVisualProviderWall = itemVisualProviderWall;
        }

        protected override void InternalOnInitialize()
        {
            _activeBarTables = new Dictionary<Vector2Int, DeckItemVisual>();
            _activeBarTableAgents = new Dictionary<Vector2Int, DeckAgent>();
            _activeWallConnections = new Dictionary<Vector2Int, Dictionary<Vector2Int, DeckItemVisual>>();
            _barTableCheckSet = new HashSet<Vector2Int>();

            DeckEventManager.Register<DeckEventOnWallBuild>(OnWallCreated);
            DeckEventManager.Register<DeckEventOnWallDestroyed>(OnWallDestroyed);
        }

        protected override void InternalOnDeInitialize()
        {
            DeckEventManager.Unregister<DeckEventOnWallBuild>(OnWallCreated);
            DeckEventManager.Unregister<DeckEventOnWallDestroyed>(OnWallDestroyed);
        }

        private void OnWallCreated(DeckEventOnWallBuild obj)
        {
            AdjustWallConnections(obj.position);
        }

        private void OnWallDestroyed(DeckEventOnWallDestroyed obj)
        {
            AdjustWallConnections(obj.position);
        }

        private void AdjustWallConnections(Vector2Int cellIndex)
        {
            var neighbours = cellIndex.GetRectNeighbours();
            foreach (var barTablePosition in neighbours)
            {
                if (!_barTableCheckSet.Contains(barTablePosition))
                {
                    continue;
                }

                var prefabSets = GetWallConnectionPrefab(barTablePosition);

                var positionsToRemove = new HashSet<Vector2Int>();
                if (_activeWallConnections.TryGetValue(barTablePosition, out var existingValues))
                {
                    foreach (var deckItemVisual in existingValues)
                    {
                        positionsToRemove.Add(deckItemVisual.Key);
                    }
                }

                foreach (var prefabSet in prefabSets)
                {
                    if (!_activeWallConnections.ContainsKey(barTablePosition))
                    {
                        _activeWallConnections[barTablePosition] = new Dictionary<Vector2Int, DeckItemVisual>();
                    }

                    if (_activeWallConnections[barTablePosition].TryGetValue(prefabSet.Item2, out var currentlyPlaced))
                    {
                        if (currentlyPlaced.PrefabId.Equals(prefabSet.Item1.PrefabId))
                        {
                            positionsToRemove.Remove(prefabSet.Item2);
                            continue;
                        }

                        ReturnItemVisual(currentlyPlaced);
                    }

                    positionsToRemove.Remove(prefabSet.Item2);

                    RentIfHasItemVisual(prefabSet.Item1.PrefabId, out var itemVisualInstance);
                    itemVisualInstance.transform.position = prefabSet.Item2.ToVector3();
                    _activeWallConnections[barTablePosition][prefabSet.Item2] = itemVisualInstance;
                }

                foreach (var positionToRemove in positionsToRemove)
                {
                    ReturnIfHasItemVisual(existingValues[positionToRemove]);
                    existingValues.Remove(positionToRemove);
                }

                // ReplaceItemVisualForWallConnection(barTablePosition);
            }
        }

        public override bool ReturnItemVisual(DeckItemVisual itemVisual)
        {
            if (!IsSupportedItemVisual(itemVisual)) return false;

            ReturnIfHasItemVisual(itemVisual);
            var itemPos = itemVisual.transform.position.ToVector2Int();

            _activeBarTables.Remove(itemPos);
            _activeBarTableAgents.Remove(itemPos);
            _barTableCheckSet.Remove(itemPos);

            return true;
        }

        public override bool RequestItemVisual(DeckAgent agent, DeckId prefabId, Vector2Int cellIndex, out DeckItemVisual itemVisual)
        {
            if (!IsSupportedItemVisual(prefabId))
            {
                itemVisual = null;
                return false;
            }

            _activeBarTableAgents[cellIndex] = agent;
            _barTableCheckSet.Add(cellIndex);
            itemVisual = PlaceBarTableWithNeighbours(cellIndex);
            return true;
        }

        private DeckItemVisual PlaceBarTableWithNeighbours(Vector2Int position)
        {
            var neighbours = position.GetNeighbours();

            foreach (var neighbour in neighbours)
            {
                if (_barTableCheckSet.Contains(neighbour))
                {
                    ReturnIfHasItemVisual(_activeBarTables[neighbour]);
                    PlaceItemVisual(neighbour);
                }

                if (_itemVisualProviderWall.HasWallOnPosition(neighbour))
                {
                    AdjustWallConnections(neighbour);
                }
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
                var isBarTable = _barTableCheckSet.Contains(neighbour) || _activeWallConnections.ContainsKey(neighbour);
                checkList[index] = isBarTable;
            }

            var agent = _activeBarTableAgents[position];
            var itemVisualPrefab = GetVisualToPlace(checkList);
            RentIfHasItemVisual(itemVisualPrefab.PrefabId, out var itemVisualInstance);
            agent.SetItemVisual(itemVisualInstance);
            itemVisualInstance.SetAgent(agent);

            _activeBarTables[position] = itemVisualInstance;

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
                    return threeCornerDownPrefab;
                }

                if (!checkList[3])
                {
                    return threeCornerUpPrefab;
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

        private List<(DeckItemVisual, Vector2Int)> GetWallConnectionPrefab(Vector2Int cellIndex)
        {
            var wallNeighbourSet = _itemVisualProviderWall.GetNeighbourWallSet(cellIndex);

            var connectionPrefab = new List<(DeckItemVisual, Vector2Int)>();

            if (wallNeighbourSet[0].Item2)
            {
                if (wallNeighbourSet[4].Item2 || wallNeighbourSet[6].Item2)
                {
                    connectionPrefab.Add(new ValueTuple<DeckItemVisual, Vector2Int>(rightWallConnectionPrefab, wallNeighbourSet[0].Item1));
                }

                if (wallNeighbourSet[3].Item2 && wallNeighbourSet[6].Item2)
                {
                    connectionPrefab.Add(new ValueTuple<DeckItemVisual, Vector2Int>(fourCornerWallConnectionLowerRightSinglePrefab, wallNeighbourSet[6].Item1));
                }

                if (wallNeighbourSet[4].Item2 && wallNeighbourSet[2].Item2)
                {
                    connectionPrefab.Add(new ValueTuple<DeckItemVisual, Vector2Int>(fourCornerWallConnectionUpperRightSinglePrefab, wallNeighbourSet[4].Item1));
                }
            }

            if (wallNeighbourSet[1].Item2)
            {
                if (wallNeighbourSet[5].Item2 || wallNeighbourSet[7].Item2)
                {
                    connectionPrefab.Add(new ValueTuple<DeckItemVisual, Vector2Int>(leftWallConnectionPrefab, wallNeighbourSet[1].Item1));
                }

                if (wallNeighbourSet[2].Item2 && wallNeighbourSet[5].Item2)
                {
                    connectionPrefab.Add(new ValueTuple<DeckItemVisual, Vector2Int>(fourCornerWallConnectionUpperLeftSinglePrefab, wallNeighbourSet[5].Item1));
                }

                if (wallNeighbourSet[3].Item2 && wallNeighbourSet[7].Item2)
                {
                    connectionPrefab.Add(new ValueTuple<DeckItemVisual, Vector2Int>(fourCornerWallConnectionLowerLeftSinglePrefab, wallNeighbourSet[7].Item1));
                }
            }

            if (wallNeighbourSet[2].Item2 && (wallNeighbourSet[4].Item2 || wallNeighbourSet[5].Item2))
            {
                connectionPrefab.Add(new ValueTuple<DeckItemVisual, Vector2Int>(upWallConnectionPrefab, wallNeighbourSet[2].Item1));
            }

            if (wallNeighbourSet[3].Item2 && (wallNeighbourSet[7].Item2 || wallNeighbourSet[6].Item2))
            {
                connectionPrefab.Add(new ValueTuple<DeckItemVisual, Vector2Int>(downWallConnectionPrefab, wallNeighbourSet[3].Item1));
            }

            return connectionPrefab;
        }
    }
}