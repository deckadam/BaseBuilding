using System.Collections.Generic;
using System.Linq;
using Base;
using Data.Buildable;
using Data.General;
using EventManager;
using InGame.Agent.Building;
using Instancing;
using Services.Camera;
using Services.Currency;
using Services.ItemVisual;
using Systems.SystemInput.Events;
using UI.Building.BuildMode;
using UI.Notification;
using UnityEngine;
using Utility;
using Utility.Iterators;
using Utility.MonoBehaviours;
using Zenject;

namespace Services.Building
{
    public class DeckServiceBuilding : DeckServiceBase
    {
        private const int LAYER_MASK = 0b_0011_1111_1_1111_1_1111_1111_1111;
        private const float YOffsetForBuildOnGround = 0.1f;
        private const float YOffsetForBuildOnTop = 0.1f;
        private const string WallTag = "Wall";

        [SerializeField] private LayerMask layerMask;
        [SerializeField] private LayerMask onWallLayerMask;

        private readonly Vector3 _cellHalfExtents = Vector3.one / 2.001f;
        private readonly Vector2Int _defaultCellPosition = new(1000000, 1000000);
        private readonly Dictionary<Vector2Int, DeckAgentBuilding> _grid = new();
        private readonly List<DeckSilhouettePiece> _piecesInUse = new();
        private readonly Dictionary<Vector2Int, int> _accessCells = new();
        private readonly Collider[] _possibleColliders = new Collider[5];
        private DeckInstanceProvider _instanceProvider;
        private Vector2Int _lastCheckedCellIndex;
        private DeckServiceCamera _cameraService;
        private DeckServiceCurrency _currencyService;
        private DeckDataBuilding _buildingData;
        private DeckBuildable _activeBuildable;
        private GameObject _silhouetteParent;
        private Quaternion _rotation;
        private bool _isDirty;
        private int _ninetyDegreeRotationAmount;
        private DeckSilhouetteProvider _silhouetteProvider;

        [Inject]
        private void Inject(DeckDataBuilding buildingData, DeckInstanceProvider instanceProvider, DeckSilhouetteProvider silhouetteProvider)
        {
            _buildingData = buildingData;
            _instanceProvider = instanceProvider;
            _silhouetteProvider = silhouetteProvider;
        }

        public override void Initialize()
        {
            _cameraService = DeckServiceProvider.GetService<DeckServiceCamera>();
            _currencyService = DeckServiceProvider.GetService<DeckServiceCurrency>();
            _lastCheckedCellIndex = _defaultCellPosition;
            _silhouetteParent = new GameObject()
            {
                name = "SilhouetteParent"
            };
        }

        protected override void DrawGizmos()
        {
            if (!showGizmos)
            {
                return;
            }

            foreach (var kvp in _grid)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawCube(kvp.Key.ToVector3(), Vector3.one * 0.8f);
            }

            foreach (var kvp in _accessCells)
            {
                if (kvp.Value > 0)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawCube(kvp.Key.ToVector3(), Vector3.one * 0.8f);
                }
            }
        }

        private void OnMiddleScroll(DeckEventMiddleScroll evt)
        {
            if (!_activeBuildable)
            {
                return;
            }

            if (_activeBuildable.RotationMode == DeckRotationMode.Continuous)
            {
                _silhouetteParent.transform.Rotate(0, -evt.scrollValue * _buildingData.GetBuildableRotationSpeed(), 0);
            }
            else if (_activeBuildable.RotationMode == DeckRotationMode.NinetyDegree)
            {
                if (evt.scrollValue > 0)
                {
                    _ninetyDegreeRotationAmount--;
                }
                else
                {
                    _ninetyDegreeRotationAmount++;
                }

                _silhouetteParent.transform.rotation = Quaternion.Euler(0, 90 * _ninetyDegreeRotationAmount, 0);
            }

            _rotation = _silhouetteParent.transform.rotation;
        }

        public void StartSilhouette(DeckBuildable buildable)
        {
            _isDirty = true;
            _activeBuildable = buildable;
            _rotation = Quaternion.identity;

            var hasIndices = false;
            if (buildable.Indices != null)
            {
                foreach (var buildableIndex in buildable.Indices)
                {
                    hasIndices = true;
                    var newPiece = _silhouetteProvider.GetSilhouettePiece(_activeBuildable);
                    _piecesInUse.Add(newPiece);

                    newPiece.gameObject.transform.SetParent(_silhouetteParent.transform);
                    newPiece.gameObject.transform.localPosition = buildableIndex.ToVector3();
                    newPiece.gameObject.transform.localRotation = _rotation;
                }
            }

            if (buildable.AccessIndices != null)
            {
                foreach (var buildableIndex in buildable.AccessIndices)
                {
                    hasIndices = true;
                    var newPiece = _silhouetteProvider.GetAccessAreaSilhouettePiece();
                    _piecesInUse.Add(newPiece);

                    newPiece.gameObject.transform.SetParent(_silhouetteParent.transform);
                    newPiece.gameObject.transform.localPosition = buildableIndex.ToVector3();
                    newPiece.gameObject.transform.localRotation = _rotation;
                }
            }

            if (!hasIndices)
            {
                var newPiece = _silhouetteProvider.GetSilhouettePiece(_activeBuildable);
                _piecesInUse.Add(newPiece);

                newPiece.gameObject.transform.SetParent(_silhouetteParent.transform);
                newPiece.gameObject.transform.localPosition = Vector3.zero;
                newPiece.gameObject.transform.localRotation = _rotation;
            }

            DeckEventManager.Register<DeckEventMiddleScroll>(OnMiddleScroll);

            if (!_silhouetteParent.activeSelf)
            {
                _silhouetteParent.gameObject.SetActive(true);
            }
        }

        public void StartSilhouetteRect(DeckBuildable buildable)
        {
            _isDirty = true;
            _activeBuildable = buildable;
            _rotation = Quaternion.identity;

            var newPiece = _silhouetteProvider.GetSilhouettePiece(_activeBuildable);
            _piecesInUse.Add(newPiece);

            var cellIndex = _cameraService.GetCursorCellIndex();

            newPiece.gameObject.transform.SetParent(_silhouetteParent.transform);
            newPiece.gameObject.transform.localPosition = cellIndex.ToVector3();

            DeckEventManager.Register<DeckEventMiddleScroll>(OnMiddleScroll);

            if (!_silhouetteParent.activeSelf)
            {
                _silhouetteParent.gameObject.SetActive(true);
            }

            UpdateSilhouetteInCellRect(new[] { cellIndex, cellIndex });
        }

        public void UpdateSilhouetteInCell(Quaternion rotation, bool canReplace)
        {
            if (!_activeBuildable)
            {
                return;
            }

            var cellIndex = _cameraService.GetCursorCellIndex();
            var isAvailable = true;
            if (!canReplace)
            {
                isAvailable = !(_grid.TryGetValue(cellIndex, out var value) && value);
            }

            var material = isAvailable ? _buildingData.GetAvailableMaterial() : _buildingData.GetUnavailableMaterial();

            ApplyMaterialToSilhouette(material);

            _silhouetteParent.transform.position = cellIndex.ToVector3();
            _silhouetteParent.transform.rotation = rotation;
        }

        public void UpdateSilhouetteWithAccess()
        {
            if (!_activeBuildable)
            {
                return;
            }

            var currentCellIndex = _cameraService.GetCursorCellIndex();
            var isAvailable = true;
            foreach (var mainCell in _activeBuildable.Indices)
            {
                if (!_grid.TryGetValue(currentCellIndex + mainCell, out var value))
                {
                    isAvailable = false;
                    break;
                }

                if (!value.BuildingData.ItemVisual.PrefabId.Equals(_activeBuildable.BuildableToPlaceOnTop.ItemVisual.PrefabId))
                {
                    isAvailable = false;
                    break;
                }
            }

            if (isAvailable && !IsViableBuildCell(_activeBuildable.AccessIndices, currentCellIndex))
            {
                isAvailable = false;
            }

            var material = isAvailable ? _buildingData.GetAvailableMaterial() : _buildingData.GetUnavailableMaterial();

            ApplyMaterialToSilhouette(material);

            _silhouetteParent.transform.position = currentCellIndex.ToVector3();
            _silhouetteParent.transform.rotation = Quaternion.Euler(0, 90 * _ninetyDegreeRotationAmount, 0);
        }

        public void UpdateSilhouetteInCellRect(Vector2Int[] cells)
        {
            if (!_activeBuildable)
            {
                return;
            }

            var rectFromPoints = cells.GetRectFromPoints();
            var necessaryCount = rectFromPoints.Count - _piecesInUse.Count;

            if (necessaryCount < 0)
            {
                for (var i = 0; i < -necessaryCount; i++)
                {
                    var lastPiece = _piecesInUse.Last();
                    _piecesInUse.RemoveAt(_piecesInUse.Count - 1);
                    _silhouetteProvider.ReturnSilhouettePieceToPool(lastPiece);
                }
            }

            for (var i = 0; i < necessaryCount; i++)
            {
                var newPiece = _silhouetteProvider.GetSilhouettePiece(_activeBuildable);
                _piecesInUse.Add(newPiece);
                newPiece.gameObject.transform.SetParent(_silhouetteParent.transform);
            }

            var isAllCellsFree = true;
            var rectArray = rectFromPoints.ToArray();
            var length = rectArray.Length;
            for (var index = 0; index < length; index++)
            {
                var cellIndex = rectArray[index];

                if (!_piecesInUse[index].gameObject.activeSelf)
                {
                    _piecesInUse[index].gameObject.SetActive(true);
                }

                _piecesInUse[index].gameObject.transform.position = cellIndex.ToVector3();

                if (isAllCellsFree && !IsViableBuildCell(_activeBuildable.Indices, cellIndex))
                {
                    isAllCellsFree = false;
                }
            }

            var material = isAllCellsFree ? _buildingData.GetAvailableMaterial() : _buildingData.GetUnavailableMaterial();
            ApplyMaterialToSilhouette(material);
        }

        public void UpdateSilhouetteFree()
        {
            if (!_activeBuildable)
            {
                return;
            }

            var worldPosition = _cameraService.GetCursorWorldPosition();
            var isPlaceable = IsCollidingWithAnotherObject(_activeBuildable, worldPosition);
            var material = isPlaceable ? _buildingData.GetAvailableMaterial() : _buildingData.GetUnavailableMaterial();

            ApplyMaterialToSilhouette(material);

            _silhouetteParent.transform.position = worldPosition;
        }

        public void UpdateSilhouetteOnWall()
        {
            if (!_activeBuildable)
            {
                return;
            }

            if (!IsTargetPointWall(out var buildPosition, out var buildRotation))
            {
                if (_silhouetteParent.activeSelf)
                {
                    _silhouetteParent.gameObject.SetActive(false);
                }

                return;
            }

            if (!_silhouetteParent.activeSelf)
            {
                _silhouetteParent.gameObject.SetActive(true);
            }

            var material = IsViableToBuildOnWall(buildPosition, out _) ? _buildingData.GetAvailableMaterial() : _buildingData.GetUnavailableMaterial();

            ApplyMaterialToSilhouette(material);

            _silhouetteParent.transform.position = buildPosition;
            _silhouetteParent.transform.eulerAngles = buildRotation;
        }

        public void UpdateSilhouetteOnTop()
        {
            if (_activeBuildable == null)
            {
                return;
            }

            if (!IsViableToPlaceOnTop(out _, out var buildPosition))
            {
                if (_silhouetteParent.activeSelf)
                {
                    _silhouetteParent.SetActive(false);
                }

                return;
            }

            if (!_silhouetteParent.activeSelf)
            {
                _silhouetteParent.SetActive(true);
            }

            var isPlaceable = !CollidesWithOtherItemsOnTop(buildPosition);

            var material = isPlaceable ? _buildingData.GetAvailableMaterial() : _buildingData.GetUnavailableMaterial();
            ApplyMaterialToSilhouette(material);

            _silhouetteParent.transform.position = buildPosition;
        }

        private void ApplyMaterialToSilhouette(Material material)
        {
            foreach (var silhouettePiece in _piecesInUse)
            {
                foreach (var rend in silhouettePiece.renderers)
                {
                    var materials = new Material[_activeBuildable.MaterialCount];
                    for (int i = 0; i < _activeBuildable.MaterialCount; i++)
                    {
                        materials[i] = material;
                    }

                    rend.sharedMaterials = materials;
                }
            }
        }

        private void ReturnAllSilhouettePiecesToPool()
        {
            foreach (var silhouettePiece in _piecesInUse)
            {
                _silhouetteProvider.ReturnSilhouettePieceToPool(silhouettePiece);
            }

            _piecesInUse.Clear();
        }

        public void BuildOnWall()
        {
            if (_activeBuildable == null)
            {
                return;
            }

            if (!IsTargetPointWall(out var buildPosition, out var buildRotation))
            {
                return;
            }

            if (!IsViableToBuildOnWall(buildPosition, out var collidedObject))
            {
                DeckLogger.Inform("Collides with object", collidedObject);
                return;
            }

            if (!PayIfCanAfford()) return;

            var newBuilding = _instanceProvider.RentAgent(_activeBuildable.Agent.PrefabId.ID).GetComponent<DeckAgentBuilding>();
            newBuilding.transform.position = buildPosition;
            newBuilding.transform.rotation = Quaternion.LookRotation(buildRotation * -1);
            newBuilding.Initialize();
            newBuilding.InitializeBuilding();
        }

        public void BuildOnTop()
        {
            if (!_activeBuildable)
            {
                return;
            }

            if (!IsViableToPlaceOnTop(out var buildingToBuildOnTop, out var buildPosition))
            {
                return;
            }

            if (CollidesWithOtherItemsOnTop(buildPosition))
            {
                return;
            }

            if (!PayIfCanAfford()) return;

            var newBuilding = _instanceProvider.RentAgent(_activeBuildable.Agent.PrefabId.ID).GetComponent<DeckAgentBuilding>();
            newBuilding.transform.position = buildPosition;
            newBuilding.transform.rotation = _rotation;
            newBuilding.Initialize();
            newBuilding.InitializeBuilding();
            buildingToBuildOnTop.AddBuildingToTop(newBuilding);
        }

        public void BuildFree()
        {
            if (_activeBuildable == null)
            {
                return;
            }

            var worldPosition = _cameraService.GetCursorWorldPosition();

            if (!IsCollidingWithAnotherObject(_activeBuildable, worldPosition))
            {
                return;
            }

            if (!PayIfCanAfford()) return;

            var newBuilding = _instanceProvider.RentAgent(_activeBuildable.Agent.PrefabId.ID).GetComponent<DeckAgentBuilding>();
            newBuilding.transform.position = worldPosition;
            newBuilding.transform.rotation = _rotation;
            newBuilding.Initialize();
            newBuilding.InitializeBuilding();
        }

        public void BuildInRect(Vector2Int[] positions)
        {
            if (_activeBuildable == null)
            {
                return;
            }

            var rectBuildPositions = positions.GetRectFromPoints();
            var isAllCellsAvailable = true;

            foreach (var rectBuildPosition in rectBuildPositions)
            {
                if (!IsViableBuildCell(_activeBuildable.Indices, rectBuildPosition))
                {
                    isAllCellsAvailable = false;
                }
            }

            if (!isAllCellsAvailable)
            {
                foreach (var piece in _piecesInUse)
                {
                    _silhouetteProvider.ReturnSilhouettePieceToPool(piece);
                }

                _piecesInUse.Clear();
                return;
            }

            if (!PayIfCanAfford()) return;

            foreach (var rectBuildPosition in rectBuildPositions)
            {
                var newBuilding = _instanceProvider.RentAgent(_activeBuildable.Agent.PrefabId.ID).GetComponent<DeckAgentBuilding>();
                newBuilding.transform.position = rectBuildPosition.ToVector3();
                newBuilding.Initialize();
                newBuilding.InitializeBuilding();
            }

            foreach (var silhouettePiece in _piecesInUse)
            {
                _silhouetteProvider.ReturnSilhouettePieceToPool(silhouettePiece);
            }

            _piecesInUse.Clear();
        }

        public void BuildInCell(bool canReplace)
        {
            if (_activeBuildable == null)
            {
                DeckLogger.Warning($"Buildable not found {_activeBuildable.name}");
                return;
            }

            var cellIndex = _cameraService.GetCursorCellIndex();

            if (_lastCheckedCellIndex == cellIndex)
            {
                return;
            }

            _lastCheckedCellIndex = cellIndex;

            if (canReplace && _grid.TryGetValue(cellIndex, out var building))
            {
                building.RequestDestroy();
            }

            if (!IsViableBuildCell(_activeBuildable.Indices, cellIndex))
            {
                return;
            }

            if (!PayIfCanAfford()) return;

            var newBuilding = _instanceProvider.RentAgent(_activeBuildable.Agent.PrefabId.ID).GetComponent<DeckAgentBuilding>();
            newBuilding.transform.position = cellIndex.ToVector3();
            newBuilding.Initialize();
            newBuilding.InitializeBuilding();
        }

        public void BuildInCellMultiple()
        {
            if (_activeBuildable == null)
            {
                DeckLogger.Error($"Buildable not found {_activeBuildable.name}");
                return;
            }

            var cellIndex = _cameraService.GetCursorCellIndex();

            if (_lastCheckedCellIndex == cellIndex)
            {
                return;
            }

            _lastCheckedCellIndex = cellIndex;

            if (!IsViableBuildCell(_activeBuildable.Indices, cellIndex))
            {
                return;
            }

            if (!PayIfCanAfford()) return;

            var newBuilding = _instanceProvider.RentAgent(_activeBuildable.Agent.PrefabId.ID).GetComponent<DeckAgentBuilding>();
            newBuilding.transform.position = cellIndex.ToVector3();
            newBuilding.Initialize();
            newBuilding.InitializeBuilding();
        }

        public void BuildInRectBulk(DeckBuildable buildable, Vector2Int min, Vector2Int max)
        {
            var cellCount = max.x * max.y;
            var indices = new Vector2Int[max.x * max.y];

            var counter_1 = 0;
            for (var x = min.x; x < max.x; x++)
            {
                for (var y = min.y; y < max.y; y++)
                {
                    indices[counter_1++] = new Vector2Int(x, y);
                }
            }

            var builtAgents = _instanceProvider.BulkRentAgent<DeckAgentBuilding>(buildable.Agent.PrefabId, cellCount);
            DeckServiceProvider.GetService<DeckServiceItemVisual>().RequestMultipleItemVisual(builtAgents, buildable.ItemVisual.PrefabId, cellCount, indices, out var rentedVisuals);

            var counter_2 = 0;
            for (var x = min.x; x < max.x; x++)
            {
                for (var y = min.y; y < max.y; y++)
                {
                    builtAgents[counter_2].transform.position = new Vector3(x, 0, y);
                    builtAgents[counter_2].InitializeBuildingWithVisual(rentedVisuals[counter_2++]);
                }
            }
        }

        public void BuildWithAccess()
        {
            var currentCellIndex = _cameraService.GetCursorCellIndex();
            DeckAgentBuilding agentBuildingToBuildOnTop = null;
            foreach (var mainCell in _activeBuildable.Indices)
            {
                if (!_grid.TryGetValue(currentCellIndex + mainCell, out agentBuildingToBuildOnTop))
                {
                    return;
                }

                if (!agentBuildingToBuildOnTop.BuildingData.Equals(_activeBuildable.BuildableToPlaceOnTop))
                {
                    return;
                }
            }

            if (agentBuildingToBuildOnTop == null)
            {
                return;
            }

            if (!IsViableBuildCell(_activeBuildable.Indices, currentCellIndex, false))
            {
                return;
            }

            if (!IsViableAccessCell(_activeBuildable.AccessIndices, currentCellIndex))
            {
                return;
            }

            if (!PayIfCanAfford())
            {
                return;
            }

            var newBuilding = _instanceProvider.RentAgent(_activeBuildable.Agent.PrefabId.ID).GetComponent<DeckAgentBuilding>();
            newBuilding.transform.position = currentCellIndex.ToVector3();
            newBuilding.transform.rotation = Quaternion.Euler(0, 90 * _ninetyDegreeRotationAmount, 0);
            newBuilding.Initialize();
            newBuilding.InitializeBuilding();

            agentBuildingToBuildOnTop.AddBuildingToTop(newBuilding);
        }

        private bool CollidesWithOtherItemsOnTop(Vector3 position)
        {
            switch (_activeBuildable.ItemVisual.Collider)
            {
                case BoxCollider boxCollider:
                {
                    var size = boxCollider.size;
                    var yOffset = new Vector3(0, boxCollider.size.y / 2f, 0);
                    return Physics.OverlapBoxNonAlloc(position + yOffset, size, _possibleColliders) > 0;
                }
                case SphereCollider sphereCollider:
                {
                    var radius = sphereCollider.radius;
                    var yOffset = new Vector3(0, radius + YOffsetForBuildOnTop, 0);
                    return Physics.OverlapSphereNonAlloc(position + yOffset, radius, _possibleColliders, layerMask, QueryTriggerInteraction.Ignore) > 0;
                }
                case CapsuleCollider capsuleCollider:
                {
                    var direction = new Vector3 { [capsuleCollider.direction] = 1 };
                    var radius = capsuleCollider.radius;
                    var offset = capsuleCollider.height / 2 - radius;
                    var yOffset = new Vector3(0, radius + YOffsetForBuildOnTop, 0);
                    var localPoint0 = capsuleCollider.center - direction * offset + yOffset;
                    var localPoint1 = capsuleCollider.center + direction * offset + yOffset;
                    var point0 = _silhouetteParent.transform.TransformPoint(localPoint0);
                    var point1 = _silhouetteParent.transform.TransformPoint(localPoint1);

                    return Physics.OverlapCapsuleNonAlloc(point0, point1, radius, _possibleColliders, layerMask, QueryTriggerInteraction.Ignore) > 0;
                }
                default:
                    DeckLogger.Error($"Not suppoerted collider type {_activeBuildable.ItemVisual.Collider.GetType()}");
                    return true;
            }
        }

        public void ClearAll()
        {
            _lastCheckedCellIndex = _defaultCellPosition;
            if (!_isDirty)
            {
                return;
            }

            if (_activeBuildable)
            {
                DeckEventManager.Unregister<DeckEventMiddleScroll>(OnMiddleScroll);
            }

            _activeBuildable = null;

            foreach (var piece in _piecesInUse)
            {
                _silhouetteProvider.ReturnSilhouettePieceToPool(piece);
            }

            _piecesInUse.Clear();
            _rotation = Quaternion.identity;

            if (_silhouetteParent && _silhouetteParent != null && _silhouetteParent.activeInHierarchy)
            {
                _silhouetteParent.transform.rotation = Quaternion.identity;
            }

            _ninetyDegreeRotationAmount = 0;
            _isDirty = false;
        }

        public void SetCellsUnoccupied(Vector2Int cellIndex, Vector2Int[] cells)
        {
            foreach (var cell in cells)
            {
                _grid.Remove(cell + cellIndex);
            }
        }

        public void SetCellsOccupied(Vector2Int cellIndex, IEnumerable<Vector2Int> indices, DeckAgentBuilding agentToSet)
        {
            foreach (var index in indices)
            {
                var temp = cellIndex + index;
                _grid[temp] = agentToSet;
            }
        }

        public void AddAccessCellReference(Vector2Int cellIndex, IEnumerable<Vector2Int> indices)
        {
            foreach (var index in indices)
            {
                var temp = cellIndex + index;
                _accessCells.TryAdd(temp, 0);
                _accessCells[temp]++;
            }
        }

        public void RemoveAccessCellReference(Vector2Int cellIndex, IEnumerable<Vector2Int> indices)
        {
            foreach (var index in indices)
            {
                var temp = cellIndex + index;
                _accessCells.TryAdd(temp, 0);
                _accessCells[temp]--;
            }
        }

        public bool[] GetCellNeighbourStatus(Vector2Int cellIndex)
        {
            var neighbours = cellIndex.GetNeighbours();
            var result = new bool[4];
            for (var index = 0; index < neighbours.Length; index++)
            {
                var temp = neighbours[index];
                if (_grid.TryGetValue(temp, out var value) && value != null)
                {
                    result[index] = true;
                }
            }

            return result;
        }

        private bool IsViableToBuildOnWall(Vector3 position, out GameObject collidedObject)
        {
            var boxCollider = _activeBuildable.ItemVisual.Collider as BoxCollider;
            if (!boxCollider)
            {
                DeckLogger.Warning("Collider is not BoxCollider");
                collidedObject = null;
                return false;
            }

            if (Physics.OverlapBoxNonAlloc(position + boxCollider.center, boxCollider.size / 2, _possibleColliders, Quaternion.identity, onWallLayerMask, QueryTriggerInteraction.Ignore) != 0)
            {
                collidedObject = null;
                return false;
            }

            if (_possibleColliders[0] == null)
            {
                collidedObject = null;
                return false;
            }

            collidedObject = _possibleColliders[0].gameObject;
            return true;
        }

        private bool IsViableBuildCell(Vector2Int[] indices, Vector2Int cellIndex, bool checkIfGridClear = true)
        {
            var rotatedIndices = indices.GetRotatedIndices(_ninetyDegreeRotationAmount);
            foreach (var index in rotatedIndices)
            {
                var temp = cellIndex + index;

                if (_accessCells.TryGetValue(temp, out var accessCount) && accessCount > 0)
                {
                    return false;
                }

                if (checkIfGridClear)
                {
                    if (!_grid.TryGetValue(temp, out var value))
                    {
                        continue;
                    }

                    if (value != null)
                    {
                        return false;
                    }
                }

                var collisionCount = Physics.OverlapBoxNonAlloc(temp.ToVector3(), _cellHalfExtents, _possibleColliders, Quaternion.identity, LAYER_MASK);

                if (collisionCount > 0)
                {
                    if (collisionCount > 1)
                    {
                        return false;
                    }

                    if (_activeBuildable.BuildMode == DeckBuildMode.BuildOnTopWithAccessArea && _possibleColliders[0].gameObject.TryGetComponentInParent<DeckAgentBuilding>(out var building))
                    {
                        if (!building.BuildingData.Equals(_activeBuildable.BuildableToPlaceOnTop))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private bool IsViableAccessCell(Vector2Int[] indices, Vector2Int cellIndex)
        {
            var rotatedIndices = indices.GetRotatedIndices(_ninetyDegreeRotationAmount);
            foreach (var index in rotatedIndices)
            {
                var temp = cellIndex + index;

                var collisionCount = Physics.OverlapBoxNonAlloc(temp.ToVector3(), _cellHalfExtents, _possibleColliders, Quaternion.identity, LAYER_MASK);

                if (collisionCount > 0)
                {
                    return false;
                }

                if (!_grid.TryGetValue(temp, out var value))
                {
                    continue;
                }

                if (value != null)
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsViableToPlaceOnTop(out DeckAgentBuilding agentBuilding, out Vector3 position)
        {
            if (Physics.Raycast(_cameraService.GetRayFromCamera(), out var hit))
            {
                if (hit.transform.TryGetComponentInParent(out agentBuilding))
                {
                    if (agentBuilding.GetItemVisual().CanBePlacedOnTop)
                    {
                        position = hit.point;
                        return true;
                    }
                }
            }

            agentBuilding = null;
            position = hit.point;
            return false;
        }

        private bool IsTargetPointWall(out Vector3 position, out Vector3 rotation)
        {
            if (Physics.Raycast(_cameraService.GetRayFromCamera(), out var hit, 1000f, layerMask, QueryTriggerInteraction.Ignore))
            {
                if (hit.transform.TryGetComponentInParent(out DeckItemVisual itemVisual))
                {
                    if (itemVisual.CompareTag(WallTag))
                    {
                        position = hit.point;
                        rotation = hit.normal;
                        return true;
                    }
                }
            }

            position = Vector3.zero;
            rotation = Vector3.zero;
            return false;
        }

        private bool IsCollidingWithAnotherObject(DeckBuildable buildable, Vector3 worldPosition)
        {
            return Physics.OverlapBoxNonAlloc(worldPosition + new Vector3(0, buildable.Extents.y / 2f + YOffsetForBuildOnGround, 0), buildable.Extents / 2f,
                _possibleColliders, _rotation, layerMask, QueryTriggerInteraction.Ignore) == 0;
        }

        private bool PayIfCanAfford()
        {
            if (!_currencyService.CanAfford(_activeBuildable.Prices))
            {
                DeckEventNotificationRequested.Create("Cant afford").Send();
                ReturnAllSilhouettePiecesToPool();
                return false;
            }

            _currencyService.ChangeValueRelative(_activeBuildable.Prices, false);
            return true;
        }
    }
}