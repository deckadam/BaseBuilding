using System.Collections.Generic;
using Deck.Components;
using Deck.Components.Building;
using Deck.Data.Buildable;
using Deck.Data.General;
using Deck.EventManager;
using Deck.InputHandling.Events;
using Deck.Save.Data;
using Deck.Services.CameraService;
using Deck.Services.CellSelectionService;
using Deck.Services.MapService;
using Deck.Utility;
using Deck.Utility.Logger;
using Deck.Utility.MonoBehaviours;
using UnityEngine;
using Zenject;

namespace Deck.Services.Building
{
    public class DeckServiceBuilding : DeckServiceBase
    {
        [SerializeField] private LayerMask layerMask;

        private const string WallTag = "Wall";
        private const float YOffsetForBuildOnTop = 0.1f;
        private readonly Vector2Int _defaultCellPosition = new Vector2Int(1000000, 1000000);
        private Dictionary<Vector2Int, DeckAgent> _grid = new();
        private Collider[] _possibleColliders = new Collider[2];
        private List<MeshRenderer> _renderers = new();
        private List<MeshFilter> _filters = new();
        private DeckDataBuilding _buildingData;
        private DeckBuildable _activeBuildable;
        private DeckServiceCamera _cameraService;
        private GameObject _silouetteParent;
        private DeckInstanceProvider _instanceProvider;
        private Vector2Int _lastCheckedCellIndex;
        private Quaternion _rotation;
        private bool _isDirty;

        [Inject]
        private void Inject(DeckBuildable[] buildables, DeckDataBuilding buildingData,
            DeckDataBuilding buildableRotationSpeed, DeckInstanceProvider instanceProvider)
        {
            _buildingData = buildingData;
            _instanceProvider = instanceProvider;
        }

        public override void Initialize()
        {
            _cameraService = Deck.GetService<DeckServiceCamera>();
            _lastCheckedCellIndex = _defaultCellPosition;
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

        private void SetActiveBuildable(DeckBuildable buildable)
        {
            Debug.LogError("set active buildable  " + buildable.name);
            _activeBuildable = buildable;
        }

        public void StartSilhouette(DeckBuildable buildable)
        {
            _isDirty = true;
            SetActiveBuildable(buildable);

            var silhouetteData = _activeBuildable.Silouette;

            foreach (var data in silhouetteData)
            {
                var newObject = new GameObject();
                var newFilter = newObject.AddComponent<MeshFilter>();
                newFilter.mesh = data.GetMesh();
                var newRenderer = newObject.AddComponent<MeshRenderer>();
                newRenderer.sharedMaterial = _buildingData.GetAvailableMaterial();

                newObject.transform.SetParent(_silouetteParent.transform);
                newObject.transform.localPosition = data.GetPosition();
                newObject.transform.localRotation = Quaternion.Euler(data.GetRotation());

                _filters.Add(newFilter);
                _renderers.Add(newRenderer);
            }

            if (buildable.Rotatable)
            {
                DeckEventManager.Register<DeckEventMiddleScroll>(OnMiddleScroll);
            }
        }

        private void OnMiddleScroll(DeckEventMiddleScroll obj)
        {
            if (!_activeBuildable)
            {
                return;
            }

            _silouetteParent.transform.Rotate(0, obj.scrollValue * _buildingData.GetBuildableRotationSpeed(), 0);
            _rotation = _silouetteParent.transform.rotation;
        }

        public void UpdateSilhouetteInCell()
        {
            if (!_activeBuildable)
            {
                return;
            }

            var cellIndex = _cameraService.GetCursorWorldPosition().ToVector2Int();
            var isPlacable = CheckIfAgentBuildableInCell(_activeBuildable, cellIndex);
            var material = isPlacable ? _buildingData.GetAvailableMaterial() : _buildingData.GetUnavailableMaterial();
            foreach (var meshRenderer in _renderers)
            {
                meshRenderer.sharedMaterial = material;
            }

            _silouetteParent.transform.position = cellIndex.ToVector3();
        }

        public void UpdateSilhouetteFree()
        {
            if (!_activeBuildable)
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

            _silouetteParent.transform.position = worldPosition;
        }

        public void UpdateSilhouetteOnWall()
        {
            if (!_activeBuildable)
            {
                return;
            }

            if (!TryGetWallCollision(out var collidedWall, out var buildPosition, out var buildRotation))
            {
                return;
            }

            var isPlacable = true;
            var material = isPlacable ? _buildingData.GetAvailableMaterial() : _buildingData.GetUnavailableMaterial();
            foreach (var meshRenderer in _renderers)
            {
                meshRenderer.sharedMaterial = material;
            }

            _silouetteParent.transform.position = buildPosition;
            _silouetteParent.transform.eulerAngles = buildRotation;
        }

        public void UpdateSilhouetteOnTop()
        {
            if (_activeBuildable == null)
            {
                return;
            }

            if (!TryGetCollidedItemVisual(out var collidedItemVisual, out var buildPosition))
            {
                return;
            }

            var isPlacable = true;
            var material = isPlacable ? _buildingData.GetAvailableMaterial() : _buildingData.GetUnavailableMaterial();
            foreach (var meshRenderer in _renderers)
            {
                meshRenderer.sharedMaterial = material;
            }

            _silouetteParent.transform.position = buildPosition;
        }

        public void BuildOnWall()
        {
            if (!_activeBuildable)
            {
                DeckLogger.Warning($"Buildable not found {_activeBuildable.name}");
                return;
            }

            if (!TryGetWallCollision(out var collidedWall, out var buildPosition, out var buildRotation))
            {
                return;
            }

            if (CollidesWithOtherItemsOnWall(buildPosition, buildRotation, out var collidedObject))
            {
                DeckLogger.Inform("Collides with object", collidedObject);
                return;
            }

            if (_activeBuildable.Requeriements.Length > 0)
            {
                var hasItems = DeckServiceSelection.currentPossession.GetDeckComponent<DeckComponentInventory>()
                    .ReduceIfPossible(_activeBuildable.Requeriements);
                if (!hasItems)
                {
                    DeckEventNotificationRequested.Create(DeckConstantsNotification.OnItemRequirementNotMet).Send();
                    return;
                }
            }

            var newBuilding = _instanceProvider.RentAgent(_activeBuildable.Agent.PrefabId.ID).GetComponent<DeckBuilding>();
            newBuilding.transform.SetParent(DeckServiceScene.GetMap().transform);
            newBuilding.transform.position = buildPosition;
            newBuilding.transform.rotation = Quaternion.LookRotation(buildRotation * -1);
            newBuilding.Initialize();
            newBuilding.InitializeBuilding();
        }

        public void BuildOnTop()
        {
            if (!_activeBuildable)
            {
                DeckLogger.Warning($"Buildable not found {_activeBuildable.name}");
                return;
            }

            if (!TryGetCollidedItemVisual(out var collidedItemVisual, out var buildPosition))
            {
                return;
            }

            if (CollidesWithOtherItemsOnTop(buildPosition))
            {
                Debug.Log("Building is already on top");
                return;
            }

            Debug.LogError("No collision");

            if (_activeBuildable.Requeriements.Length > 0)
            {
                var hasItems = DeckServiceSelection.currentPossession.GetDeckComponent<DeckComponentInventory>()
                    .ReduceIfPossible(_activeBuildable.Requeriements);
                if (!hasItems)
                {
                    DeckEventNotificationRequested.Create(DeckConstantsNotification.OnItemRequirementNotMet).Send();
                    return;
                }
            }

            var newBuilding = _instanceProvider.RentAgent(_activeBuildable.Agent.PrefabId.ID).GetComponent<DeckBuilding>();
            newBuilding.transform.SetParent(DeckServiceScene.GetMap().transform);
            newBuilding.transform.position = buildPosition;
            newBuilding.transform.rotation = _rotation;
            newBuilding.Initialize();
            newBuilding.InitializeBuilding();
        }

        public void BuildFree(Vector3 position)
        {
            Debug.LogError(_activeBuildable.name);
            Debug.LogError(_activeBuildable.Agent.name);
            Debug.LogError(_activeBuildable.ItemVisual.name);
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
                var hasItems = DeckServiceSelection.currentPossession.GetDeckComponent<DeckComponentInventory>()
                    .ReduceIfPossible(_activeBuildable.Requeriements);
                if (!hasItems)
                {
                    DeckEventNotificationRequested.Create(DeckConstantsNotification.OnItemRequirementNotMet).Send();
                    return;
                }
            }

            var newBuilding = _instanceProvider.RentAgent(_activeBuildable.Agent.PrefabId.ID).GetComponent<DeckBuilding>();
            newBuilding.transform.SetParent(DeckServiceScene.GetMap().transform);
            newBuilding.transform.position = position;
            newBuilding.transform.rotation = _rotation;
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

            var cellPosition = position.ToVector2Int();

            if (_lastCheckedCellIndex == cellPosition)
            {
                return;
            }

            _lastCheckedCellIndex = cellPosition;

            if (!CheckIfAgentBuildableInCell(_activeBuildable, position.ToVector2Int(), out var encounteredAgents))
            {
                foreach (var encounteredAgent in encounteredAgents)
                {
                    if (encounteredAgent.PrefabId.Equals(_activeBuildable.Agent.PrefabId))
                    {
                        return;
                    }

                    encounteredAgent.RequestDestroy();
                }
            }

            if (_activeBuildable.Requeriements.Length > 0)
            {
                var hasItems = DeckServiceSelection.currentPossession.GetDeckComponent<DeckComponentInventory>()
                    .ReduceIfPossible(_activeBuildable.Requeriements);
                if (!hasItems)
                {
                    DeckEventNotificationRequested.Create(DeckConstantsNotification.OnItemRequirementNotMet).Send();
                    return;
                }
            }

            var newBuilding = _instanceProvider.RentAgent(_activeBuildable.Agent.PrefabId.ID).GetComponent<DeckBuilding>();
            newBuilding.transform.SetParent(DeckServiceScene.GetMap().transform);
            newBuilding.transform.position = cellPosition.ToVector3();
            newBuilding.Initialize();
            newBuilding.InitializeBuilding();
            SetCellOccupied(cellPosition.ToVector3(), _activeBuildable.Indices, newBuilding);
        }

        private bool CollidesWithOtherItemsOnWall(Vector3 position, Vector3 normal, out GameObject collidedObject)
        {
            var boxCollider = _activeBuildable.ItemVisual.Collider as BoxCollider;
            if (!boxCollider)
            {
                DeckLogger.Warning("Collider is not BoxCollider");
                collidedObject = null;
                return true;
            }

            if (Physics.OverlapBoxNonAlloc(position + boxCollider.center, boxCollider.size / 2, _possibleColliders, Quaternion.identity, layerMask, QueryTriggerInteraction.Ignore) > 0)
            {
                collidedObject = _possibleColliders[0].gameObject;
                return true;
            }

            collidedObject = null;
            return false;
        }

        private bool CollidesWithOtherItemsOnTop(Vector3 position)
        {
            if (_activeBuildable.ItemVisual.Collider is BoxCollider boxCollider)
            {
                var size = boxCollider.size;
                var yOffset = new Vector3(0, boxCollider.size.y / 2f, 0);
                if (Physics.OverlapBoxNonAlloc(position + yOffset, size, _possibleColliders) > 0)
                {
                    Debug.LogError($"Collides with {_possibleColliders[0].name}");
                    return true;
                }

                return false;
            }


            if (_activeBuildable.ItemVisual.Collider is SphereCollider sphereCollider)
            {
                var radius = sphereCollider.radius;
                var yOffset = new Vector3(0, radius + YOffsetForBuildOnTop, 0);
                if (Physics.OverlapSphereNonAlloc(position + yOffset, radius, _possibleColliders, layerMask, QueryTriggerInteraction.Ignore) > 0)
                {
                    Debug.LogError($"Collides with {_possibleColliders[0].name}");
                    return true;
                }

                return false;
            }

            DeckLogger.Error($"Not suppoerted collider type {_activeBuildable.ItemVisual.Collider.GetType()}");

            return true;
        }

        public void Clear()
        {
            _lastCheckedCellIndex = _defaultCellPosition;
            if (!_isDirty)
            {
                return;
            }

            if (_activeBuildable && _activeBuildable.Rotatable)
            {
                DeckEventManager.Unregister<DeckEventMiddleScroll>(OnMiddleScroll);
            }

            _activeBuildable = null;

            foreach (var meshFilter in _filters)
            {
                if (meshFilter)
                {
                    Destroy(meshFilter.gameObject);
                }
            }

            _filters.Clear();
            _renderers.Clear();
            _rotation = Quaternion.identity;
            _isDirty = false;
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

        private bool CheckIfAgentBuildableInCell(DeckBuildable buildable, Vector2Int cellIndex,
            out HashSet<DeckAgent> encounteredAgents)
        {
            encounteredAgents = new HashSet<DeckAgent>();
            foreach (var index in buildable.Indices)
            {
                var temp = cellIndex + index;
                if (!_grid.ContainsKey(temp))
                {
                    continue;
                }

                if (_grid[temp] != null)
                {
                    encounteredAgents.Add(_grid[temp]);
                }
            }

            return encounteredAgents.Count <= 0;
        }

        private bool TryGetCollidedItemVisual(out DeckItemVisual itemVisual, out Vector3 position)
        {
            if (Physics.Raycast(GetRayFromCamera(), out var hit))
            {
                if (hit.transform.TryGetComponentInParent(out itemVisual))
                {
                    if (itemVisual.CanBePlacedOnTop)
                    {
                        position = hit.point;
                        return true;
                    }
                }
            }

            itemVisual = null;
            position = Vector3.zero;
            return false;
        }


        private bool TryGetWallCollision(out DeckItemVisual itemVisual, out Vector3 position, out Vector3 rotation)
        {
            if (Physics.Raycast(GetRayFromCamera(), out var hit, 1000f, layerMask, QueryTriggerInteraction.Ignore))
            {
                if (hit.transform.TryGetComponentInParent(out itemVisual))
                {
                    if (itemVisual.CompareTag(WallTag))
                    {
                        Debug.DrawLine(hit.point, hit.point + hit.normal, Color.red);
                        position = hit.point;
                        rotation = hit.normal;
                        return true;
                    }
                }
            }

            itemVisual = null;
            position = Vector3.zero;
            rotation = Vector3.zero;
            return false;
        }

        private bool CheckIfAgentBuildableInArea(DeckBuildable buildable)
        {
            var size = Physics.OverlapBoxNonAlloc(_cameraService.GetCursorWorldPosition(), buildable.Extents / 2f,
                _possibleColliders, _rotation, layerMask, QueryTriggerInteraction.Ignore);

            return size <= 0;
        }

        private Ray GetRayFromCamera()
        {
            return _cameraService.GetCamera().ScreenPointToRay(Input.mousePosition);
        }

        public Quaternion GetRotation()
        {
            return _rotation;
        }
    }
}