using System;
using System.Collections.Generic;
using Deck.Agent;
using Deck.Commands;
using Deck.Data.Buildable;
using Deck.Data.General;
using Deck.Services.CellSelectionService;
using Deck.Services.MapService;
using Deck.UI;
using Deck.Utility.Logger;
using UnityEngine;
using Deck.Utility;
using Zenject;

namespace Deck.Services.Building
{
    public class DeckServiceBuilding : DeckServiceBase
    {
#if UNITY_EDITOR
        public bool showGizmos = true;
#endif
        private Dictionary<Vector2Int, DeckAgent> _grid = new();
        private Dictionary<string, DeckBuildable> _buildableDictionary;
        private List<MeshRenderer> _renderers = new();
        private List<MeshFilter> _filters = new();
        private DeckDataBuilding _buildingData;
        private DeckBuildable _activeBuildable;
        private GameObject _silouetteMaster;
        private DeckBuildable[] _buildables;
        private DiContainer _container;

        [Inject]
        private void Inject(DeckBuildable[] buildables, DeckDataBuilding buildingData, DiContainer container)
        {
            _buildables = buildables;
            _buildingData = buildingData;
            _container = container;
        }

        private void Awake()
        {
            _silouetteMaster = new GameObject();
            _buildableDictionary = new Dictionary<string, DeckBuildable>();
            foreach (var deckBuildable in _buildables)
            {
                _buildableDictionary[deckBuildable.Name] = deckBuildable;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!showGizmos)
            {
                return;
            }

            foreach (var kvp in _grid)
            {
                Gizmos.color = kvp.Value == null ? Color.red : Color.green;
                Gizmos.DrawCube(kvp.Key.ToVector3(), Vector3.one * 0.8f);
            }
        }
#endif

        public void StartSilouette(string id)
        {
            var buildable = GetBuildable(id);
            if (buildable == null)
            {
                DeckLogger.Warning($"Buildable not found {id}");
                return;
            }

            _activeBuildable = buildable;

            var silouetteData = _activeBuildable.Silouette;
            for (var i = 0; i < silouetteData.Length; i++)
            {
                var data = silouetteData[i];
                var newObject = new GameObject();
                var newFilter = newObject.AddComponent<MeshFilter>();
                newFilter.mesh = data.GetMesh();
                var newRenderer = newObject.AddComponent<MeshRenderer>();
                newRenderer.sharedMaterial = _buildingData.GetAvailableMaterial();

                newObject.transform.SetParent(_silouetteMaster.transform);
                newObject.transform.localPosition = data.GetPosition();
                newObject.transform.eulerAngles = data.GetRotation();

                _filters.Add(newFilter);
                _renderers.Add(newRenderer);
            }
        }

        public void UpdateSilouette(Vector3 worldPosition)
        {
            if (_activeBuildable == null)
            {
                return;
            }

            var cellIndex = worldPosition.ToVector2Int();
            var isPlacable = CheckIfAgentBuildableInArea(_activeBuildable, cellIndex);
            var material = isPlacable ? _buildingData.GetAvailableMaterial() : _buildingData.GetUnavailableMaterial();
            foreach (var meshRenderer in _renderers)
            {
                meshRenderer.sharedMaterial = material;
            }

            _silouetteMaster.transform.position = cellIndex.ToVector3();
        }

        public void Build(Vector3 position)
        {
            if (_activeBuildable == null)
            {
                DeckLogger.Warning($"Buildable not found {_activeBuildable.name}");
                return;
            }

            var cellPosition = position.ToVector3Int();
            StopSilouette();
            if (!CheckIfAgentBuildableInArea(_activeBuildable, position.ToVector2Int()))
            {
                DeckNotificationRequestedEvent.Create(DeckConstantsNotification.OnBuildingAreaIsNotClear).Send();
                return;
            }

            var hasItems = DeckServiceSelection.currentPossession.GetDeckComponent<DeckComponentInventory>().ReduceIfPossible(_activeBuildable.Requeriements);
            if (!hasItems)
            {
                DeckNotificationRequestedEvent.Create(DeckConstantsNotification.OnItemRequirementNotMet).Send();
                return;
            }

            var newBuilding = _container.InstantiatePrefab(_activeBuildable.Agent).GetComponent<DeckBuilding>();
            newBuilding.transform.SetParent(DeckServiceScene.GetMap().transform);
            newBuilding.transform.position = cellPosition;
            newBuilding.Initialize();
            newBuilding.InitializeBuilding();
            SetCellOccupied(cellPosition, _activeBuildable.Indices, newBuilding);
        }

        private void StopSilouette()
        {
            foreach (var meshFilter in _filters)
            {
                Destroy(meshFilter.gameObject);
            }

            _filters.Clear();
            _renderers.Clear();
        }

        public void OnBuildingDestroyed(DeckBuilding buildable)
        {
            var cellIndex = buildable.transform.position.ToVector2Int();
            foreach (var index in buildable.BuildingData.Indices)
            {
                var temp = cellIndex + index;
                _grid[temp] = null;
            }
        }

        public void SetCellOccupied(Vector3 cellPosition, IEnumerable<Vector2Int> indices, DeckAgent newAgent)
        {
            var cellIndex = cellPosition.ToVector2Int();

            foreach (var index in indices)
            {
                var temp = cellIndex + index;
                _grid[temp] = newAgent;
            }
        }

        private bool CheckIfAgentBuildableInArea(DeckBuildable buildable, Vector2Int cellIndex)
        {
            foreach (var index in buildable.Indices)
            {
                var temp = cellIndex + index;
                if (!_grid.ContainsKey(temp))
                {
                    continue;
                }

                if (_grid[temp] != null)
                {
                    return false;
                }
            }

            return true;
        }

        private DeckBuildable GetBuildable(string name)
        {
            if (_buildableDictionary.TryGetValue(name, out var match))
            {
                return match;
            }

            return null;
        }
    }
}