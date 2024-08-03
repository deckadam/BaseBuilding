using System.Collections.Generic;
using Deck.Agent;
using Deck.Commands;
using Deck.Data.Buildable;
using Deck.Data.General;
using Deck.EventManager;
using Deck.InputHandling.Events;
using Deck.Services.CameraService;
using Deck.Services.CellSelectionService;
using Deck.Services.MapService;
using Deck.UI;
using Deck.Utility;
using Deck.Utility.Logger;
using UnityEngine;
using Zenject;

namespace Deck.Services.Building
{
    public class DeckServiceBuilding : DeckServiceBase
    {
        private Dictionary<Vector2Int, DeckAgent> _grid = new();
        private List<MeshRenderer> _renderers = new();
        private List<MeshFilter> _filters = new();
        private DeckDataBuilding _buildingData;
        private DeckBuildable _activeBuildable;
        private DeckServiceCamera _cameraService;
        private GameObject _silouetteParent;
        private DiContainer _container;
        private Quaternion _rotation;
        private bool _rotatable;
        private bool _cellBased;

        [Inject]
        private void Inject(DeckBuildable[] buildables, DeckDataBuilding buildingData, DeckDataBuilding buildableRotationSpeed, DiContainer container)
        {
            _buildingData = buildingData;
            _container = container;
        }

        public override void Initialize()
        {
            _cameraService = Deck.GetService<DeckServiceCamera>();
        }

        public override void DeInitialize()
        {
            Clear();
        }

        private void Awake()
        {
            _silouetteParent = new GameObject()
            {
                name = "SilhouetteParent"
            };
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

        public void StartSilouette(DeckBuildable buildable, bool rotatable = false, bool cellBased = false)
        {
            _activeBuildable = buildable;
            _cellBased = cellBased;

            var silouetteData = _activeBuildable.Silouette;

            foreach (var data in silouetteData)
            {
                var newObject = new GameObject();
                var newFilter = newObject.AddComponent<MeshFilter>();
                newFilter.mesh = data.GetMesh();
                var newRenderer = newObject.AddComponent<MeshRenderer>();
                newRenderer.sharedMaterial = _buildingData.GetAvailableMaterial();

                newObject.transform.SetParent(_silouetteParent.transform);
                newObject.transform.localPosition = data.GetPosition();
                newObject.transform.eulerAngles = data.GetRotation();

                _filters.Add(newFilter);
                _renderers.Add(newRenderer);
            }

            _rotatable = rotatable;

            if (_rotatable)
            {
                DeckEventManager.Register<DeckEventMiddleScroll>(OnMiddleScroll);
            }
        }

        private void OnMiddleScroll(DeckEventMiddleScroll obj)
        {
            if (_activeBuildable == null)
            {
                return;
            }

            _silouetteParent.transform.Rotate(0, obj.scrollValue * _buildingData.GetBuildableRotationSpeed(), 0);
            _rotation = _silouetteParent.transform.rotation;
        }

        public void UpdateSilouette()
        {
            if (_activeBuildable == null)
            {
                return;
            }

            var worldPosition = _cameraService.GetCursorWorldPosition();
            var cellIndex = _cameraService.GetCursorWorldPosition().ToVector2Int();
            var isPlacable = CheckIfAgentBuildableInCell(_activeBuildable, cellIndex);
            var material = isPlacable ? _buildingData.GetAvailableMaterial() : _buildingData.GetUnavailableMaterial();
            foreach (var meshRenderer in _renderers)
            {
                meshRenderer.sharedMaterial = material;
            }

            if (_cellBased)
            {
                _silouetteParent.transform.position = cellIndex.ToVector3();
            }
            else
            {
                _silouetteParent.transform.position = worldPosition;
            }
        }

        public void BuildFree(Vector3 position, Quaternion rotation)
        {
            if (_activeBuildable == null)
            {
                DeckLogger.Warning($"Buildable not found {_activeBuildable.name}");
                return;
            }

            if (!CheckIfAgentBuildableInArea(_activeBuildable))
            {
                DeckEventNotificationRequested.Create(DeckConstantsNotification.OnBuildingAreaIsNotClear).Send();
                return;
            }

            if (_activeBuildable.Requeriements.Length > 0)
            {
                var hasItems = DeckServiceSelection.currentPossession.GetDeckComponent<DeckComponentInventory>().ReduceIfPossible(_activeBuildable.Requeriements);
                if (!hasItems)
                {
                    DeckEventNotificationRequested.Create(DeckConstantsNotification.OnItemRequirementNotMet).Send();
                    return;
                }
            }

            var newBuilding = _container.InstantiatePrefab(_activeBuildable.Agent).GetComponent<DeckBuilding>();
            newBuilding.transform.SetParent(DeckServiceScene.GetMap().transform);
            newBuilding.transform.position = position;
            newBuilding.transform.rotation = rotation;
            newBuilding.Initialize();
            newBuilding.InitializeBuilding();
        }

        public void BuildInCell(Vector3 position)
        {
            if (_activeBuildable == null)
            {
                DeckLogger.Warning($"Buildable not found {_activeBuildable.name}");
                return;
            }

            var cellPosition = position.ToVector3Int();
            if (!CheckIfAgentBuildableInCell(_activeBuildable, position.ToVector2Int()))
            {
                DeckEventNotificationRequested.Create(DeckConstantsNotification.OnBuildingAreaIsNotClear).Send();
                return;
            }

            if (_activeBuildable.Requeriements.Length > 0)
            {
                var hasItems = DeckServiceSelection.currentPossession.GetDeckComponent<DeckComponentInventory>().ReduceIfPossible(_activeBuildable.Requeriements);
                if (!hasItems)
                {
                    DeckEventNotificationRequested.Create(DeckConstantsNotification.OnItemRequirementNotMet).Send();
                    return;
                }
            }

            var newBuilding = _container.InstantiatePrefab(_activeBuildable.Agent).GetComponent<DeckBuilding>();
            newBuilding.transform.SetParent(DeckServiceScene.GetMap().transform);
            newBuilding.transform.position = cellPosition;
            newBuilding.Initialize();
            newBuilding.InitializeBuilding();
            SetCellOccupied(cellPosition, _activeBuildable.Indices, newBuilding);
        }

        public void Clear()
        {
            if (_rotatable)
            {
                DeckEventManager.Unregister<DeckEventMiddleScroll>(OnMiddleScroll);
            }

            _activeBuildable = null;

            foreach (var meshFilter in _filters)
            {
                Destroy(meshFilter.gameObject);
            }

            _filters.Clear();
            _renderers.Clear();
            _rotatable = false;
            _rotation = Quaternion.identity;
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

        private bool CheckIfAgentBuildableInCell(DeckBuildable buildable, Vector2Int cellIndex)
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

        private Collider[] _possibleColliders = new Collider[2];
        public LayerMask layerMask;

        private bool CheckIfAgentBuildableInArea(DeckBuildable buildable)
        {
            var size = Physics.OverlapBoxNonAlloc(_cameraService.GetCursorWorldPosition(), buildable.Extents / 2f, _possibleColliders, _rotation, layerMask, QueryTriggerInteraction.Ignore);
            if (size <= 0) return true;

            for (var i = 0; i < size; i++)
            {
                Debug.LogError(_possibleColliders[i].name);
            }

            return false;
        }

        public Quaternion GetRotation()
        {
            return _rotation;
        }
    }
}