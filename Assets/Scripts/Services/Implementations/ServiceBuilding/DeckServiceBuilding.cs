using System.Collections.Generic;
using System.Linq;
using Deck.Components;
using Deck.Components.Building;
using Deck.Data.Buildable;
using Deck.Data.General;
using Deck.EventManager;
using Deck.InputHandling.Events;
using Deck.Save;
using Deck.Services.CameraService;
using Deck.Services.MapService;
using Deck.Utility;
using Deck.Utility.Logger;
using Deck.Utility.MonoBehaviours;
using Services.Implementations.Currency;
using UnityEngine;
using Zenject;

namespace Deck.Services.Building
{
    public class DeckServiceBuilding : DeckServiceBase
    {
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private LayerMask onWallLayerMask;

        private const string WallTag = "Wall";
        private const float YOffsetForBuildOnTop = 0.1f;
        private const float YOffsetForBuildOnGround = 0.1f;

        private readonly Vector2Int _defaultCellPosition = new(1000000, 1000000);
        private readonly Dictionary<Vector2Int, DeckAgent> _grid = new();
        private readonly Collider[] _possibleColliders = new Collider[2];
        private readonly Dictionary<int, Stack<SilhouettePiece>> _pieceInPool = new();
        private readonly List<SilhouettePiece> _pieceInUse = new();
        private readonly Vector3 _yOffset = new Vector3(0f, YOffsetForBuildOnGround, 0f);
        private DeckInstanceProvider _instanceProvider;
        private Vector2Int _lastCheckedCellIndex;
        private DeckServiceCamera _cameraService;
        private DeckServiceCurrency _currencyService;
        private DeckDataBuilding _buildingData;
        private DeckBuildable _activeBuildable;
        private GameObject _silhouetteParent;
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
            _currencyService = Deck.GetService<DeckServiceCurrency>();
            _lastCheckedCellIndex = _defaultCellPosition;
        }

        private void Awake()
        {
            _silhouetteParent = new GameObject()
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

        private void OnMiddleScroll(DeckEventMiddleScroll obj)
        {
            if (!_activeBuildable)
            {
                return;
            }

            _silhouetteParent.transform.Rotate(0, obj.scrollValue * _buildingData.GetBuildableRotationSpeed(), 0);
            _rotation = _silhouetteParent.transform.rotation;
        }

        private void SetActiveBuildable(DeckBuildable buildable)
        {
            _activeBuildable = buildable;
        }

        public void StartSilhouette(DeckBuildable buildable)
        {
            _isDirty = true;
            SetActiveBuildable(buildable);

            var newPiece = GetSilhouettePiece();

            newPiece.gameObject.transform.SetParent(_silhouetteParent.transform);
            newPiece.gameObject.transform.localPosition = Vector3.zero;
            newPiece.gameObject.transform.localRotation = _rotation;
            if (buildable.Rotatable)
            {
                DeckEventManager.Register<DeckEventMiddleScroll>(OnMiddleScroll);
            }
        }

        public void StartSilhouetteRect(DeckBuildable buildable)
        {
            _isDirty = true;
            SetActiveBuildable(buildable);

            var newPiece = GetSilhouettePiece();

            newPiece.gameObject.transform.SetParent(_silhouetteParent.transform);
            newPiece.gameObject.transform.localPosition = Vector3.zero;

            if (buildable.Rotatable)
            {
                DeckEventManager.Register<DeckEventMiddleScroll>(OnMiddleScroll);
            }
        }

        public void UpdateSilhouetteInCell()
        {
            if (!_activeBuildable)
            {
                return;
            }

            var cellIndex = _cameraService.GetCursorWorldPosition().ToVector2Int();
            var isPlaceable = CheckIfAgentBuildableInCell(_activeBuildable, cellIndex);
            var material = isPlaceable ? _buildingData.GetAvailableMaterial() : _buildingData.GetUnavailableMaterial();

            ApplyMaterialToSilhouette(material);

            _silhouetteParent.transform.position = cellIndex.ToVector3();
        }

        public void UpdateSilhouetteInCellRect(Vector3[] cells)
        {
            if (!_activeBuildable)
            {
                return;
            }

            var rectFromPoints = cells.GetRectFromPoints();
            var necessaryCount = rectFromPoints.Count - _pieceInUse.Count;

            if (necessaryCount < 0)
            {
                for (var i = 0; i < -necessaryCount; i++)
                {
                    var lastPiece = _pieceInUse.Last();
                    _pieceInUse.RemoveAt(_pieceInUse.Count - 1);
                    ReturnSilhouettePieceToPool(lastPiece);
                }
            }

            for (var i = 0; i < necessaryCount; i++)
            {
                var newPiece = GetSilhouettePiece();
                newPiece.gameObject.transform.SetParent(_silhouetteParent.transform);
            }

            var isAllCellsFree = true;
            for (var index = 0; index < rectFromPoints.Count; index++)
            {
                var rectFromPoint = rectFromPoints[index];

                if (!_pieceInUse[index].gameObject.activeSelf)
                {
                    _pieceInUse[index].gameObject.SetActive(true);
                }

                _pieceInUse[index].gameObject.transform.position = rectFromPoint.ToVector3();

                if (isAllCellsFree && _grid.ContainsKey(rectFromPoint) && _grid[rectFromPoint] != null)
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
            var cellIndex = _cameraService.GetCursorWorldPosition().ToVector2Int();
            var isPlaceable = CheckIfAgentBuildableInCell(_activeBuildable, cellIndex);
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

            if (!TryGetWallCollision(out var collidedItem, out var buildPosition, out var buildRotation))
            {
                return;
            }

            var material = !CollidesWithOtherItemsOnWall(buildPosition, out var collidedObject) ? _buildingData.GetAvailableMaterial() : _buildingData.GetUnavailableMaterial();

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

            if (!TryGetCollidedItemVisualOnTop(out var itemVisual, out var buildPosition))
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
                return;
            }

            var isPlaceable = !CollidesWithOtherItemsOnTop(buildPosition);

            var material = isPlaceable ? _buildingData.GetAvailableMaterial() : _buildingData.GetUnavailableMaterial();
            ApplyMaterialToSilhouette(material);

            _silhouetteParent.transform.position = buildPosition;
        }

        public void BuildOnWall()
        {
            if (_activeBuildable == null)
            {
                DeckLogger.Warning("Active buildable is null");
                return;
            }

            if (!_currencyService.CanAfford(_activeBuildable.Prices))
            {
                DeckEventNotificationRequested.Create("Cant afford").Send();
                ReturnAllSilhouettePiecesToPool();
                return;
            }

            _currencyService.ChangeValueRelative(_activeBuildable.Prices, false);

            if (!TryGetWallCollision(out _, out var buildPosition, out var buildRotation))
            {
                return;
            }

            if (CollidesWithOtherItemsOnWall(buildPosition, out var collidedObject))
            {
                DeckLogger.Inform("Collides with object", collidedObject);
                return;
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

            if (!_currencyService.CanAfford(_activeBuildable.Prices))
            {
                DeckEventNotificationRequested.Create("Cant afford").Send();
                ReturnAllSilhouettePiecesToPool();
                return;
            }

            _currencyService.ChangeValueRelative(_activeBuildable.Prices, false);

            if (!TryGetCollidedItemVisualOnTop(out var item, out var buildPosition))
            {
                return;
            }

            if (CollidesWithOtherItemsOnTop(buildPosition))
            {
                DeckLogger.Inform("Can't build on position");
                return;
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
            if (_activeBuildable == null)
            {
                DeckLogger.Warning($"Buildable not found {_activeBuildable.name}");
                return;
            }

            if (!_currencyService.CanAfford(_activeBuildable.Prices))
            {
                DeckEventNotificationRequested.Create("Cant afford").Send();
                ReturnAllSilhouettePiecesToPool();
                return;
            }

            _currencyService.ChangeValueRelative(_activeBuildable.Prices, false);

            if (!CheckIfAgentBuildableInArea(_activeBuildable))
            {
                DeckEventNotificationRequested.Create(DeckConstantsNotification.OnBuildingAreaIsNotClear).Send();
                return;
            }

            var newBuilding = _instanceProvider.RentAgent(_activeBuildable.Agent.PrefabId.ID).GetComponent<DeckBuilding>();
            newBuilding.transform.SetParent(DeckServiceScene.GetMap().transform);
            newBuilding.transform.position = position;
            newBuilding.transform.rotation = _rotation;
            newBuilding.Initialize();
            newBuilding.InitializeBuilding();
        }

        public void BuildInCellRect(Vector3[] position)
        {
            if (_activeBuildable == null)
            {
                DeckLogger.Warning($"Buildable not found {_activeBuildable.name}");
                return;
            }

            if (position.Length < 2)
            {
                DeckLogger.Error("Not enough positions provided: " + position.Length);
            }

            if (position.Length > 2)
            {
                DeckLogger.Error("Too much positions provided: " + position.Length);
            }

            var rectBuildPositions = position.GetRectFromPoints();
            var isAllCellsAvailable = true;

            foreach (var rectBuildPosition in rectBuildPositions)
            {
                if (!CheckIfAgentBuildableInCell(_activeBuildable, rectBuildPosition, out var encounteredAgents))
                {
                    isAllCellsAvailable = false;
                }
            }

            if (!isAllCellsAvailable)
            {
                DeckLogger.Inform("Not all cells are free");

                foreach (var piece in _pieceInUse)
                {
                    ReturnSilhouettePieceToPool(piece);
                }

                _pieceInUse.Clear();
                return;
            }

            if (!_currencyService.CanAfford(_activeBuildable.Prices, rectBuildPositions.Count))
            {
                DeckEventNotificationRequested.Create("Cant afford").Send();
                ReturnAllSilhouettePiecesToPool();
                return;
            }

            _currencyService.ChangeValueRelative(_activeBuildable.Prices, rectBuildPositions.Count, false);

            foreach (var rectBuildPosition in rectBuildPositions)
            {
                var newBuilding = _instanceProvider.RentAgent(_activeBuildable.Agent.PrefabId.ID).GetComponent<DeckBuilding>();
                newBuilding.transform.SetParent(DeckServiceScene.GetMap().transform);
                newBuilding.transform.position = rectBuildPosition.ToVector3();
                newBuilding.Initialize();
                newBuilding.InitializeBuilding();
                SetCellOccupied(rectBuildPosition, _activeBuildable.Indices, newBuilding);
            }

            foreach (var silhouettePiece in _pieceInUse)
            {
                ReturnSilhouettePieceToPool(silhouettePiece);
            }

            _pieceInUse.Clear();
        }

        public void BuildInCell(Vector2Int cellIndex)
        {
            if (_activeBuildable == null)
            {
                DeckLogger.Warning($"Buildable not found {_activeBuildable.name}");
                return;
            }

            if (!_currencyService.CanAfford(_activeBuildable.Prices))
            {
                DeckEventNotificationRequested.Create("Cant afford").Send();
                ReturnAllSilhouettePiecesToPool();
                return;
            }

            _currencyService.ChangeValueRelative(_activeBuildable.Prices, false);


            if (_lastCheckedCellIndex == cellIndex)
            {
                return;
            }

            _lastCheckedCellIndex = cellIndex;

            if (!CheckIfAgentBuildableInCell(_activeBuildable, cellIndex, out var encounteredAgents))
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

            var newBuilding = _instanceProvider.RentAgent(_activeBuildable.Agent.PrefabId.ID).GetComponent<DeckBuilding>();
            newBuilding.transform.SetParent(DeckServiceScene.GetMap().transform);
            newBuilding.transform.position = cellIndex.ToVector3();
            newBuilding.Initialize();
            newBuilding.InitializeBuilding();
            SetCellOccupied(cellIndex, _activeBuildable.Indices, newBuilding);
        }

        private bool CollidesWithOtherItemsOnWall(Vector3 position, out GameObject collidedObject)
        {
            var boxCollider = _activeBuildable.ItemVisual.Collider as BoxCollider;
            if (!boxCollider)
            {
                DeckLogger.Warning("Collider is not BoxCollider");
                collidedObject = null;
                return true;
            }

            if (Physics.OverlapBoxNonAlloc(position + boxCollider.center, boxCollider.size / 2, _possibleColliders, Quaternion.identity, onWallLayerMask, QueryTriggerInteraction.Ignore) > 0)
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
                return Physics.OverlapBoxNonAlloc(position + yOffset, size, _possibleColliders) > 0;
            }

            if (_activeBuildable.ItemVisual.Collider is SphereCollider sphereCollider)
            {
                var radius = sphereCollider.radius;
                var yOffset = new Vector3(0, radius + YOffsetForBuildOnTop, 0);
                return Physics.OverlapSphereNonAlloc(position + yOffset, radius, _possibleColliders, layerMask, QueryTriggerInteraction.Ignore) > 0;
            }

            if (_activeBuildable.ItemVisual.Collider is CapsuleCollider capsuleCollider)
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

            foreach (var piece in _pieceInUse)
            {
                ReturnSilhouettePieceToPool(piece);
            }

            _pieceInUse.Clear();
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

        public void SetCellOccupied(Vector2Int cellIndex, IEnumerable<Vector2Int> indices, DeckAgent newAgent)
        {
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

        private bool TryGetCollidedItemVisualOnTop(out DeckItemVisual itemVisual, out Vector3 position)
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
            position = hit.point;
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
                        // Debug.DrawLine(hit.point, hit.point + hit.normal, Color.red);
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
            var count = Physics.OverlapBoxNonAlloc(_cameraService.GetCursorWorldPosition() + new Vector3(0, buildable.Extents.y / 2f + YOffsetForBuildOnGround, 0), buildable.Extents / 2f,
                _possibleColliders, _rotation, layerMask, QueryTriggerInteraction.Ignore);

            return count == 0;
        }

        private Ray GetRayFromCamera()
        {
            return _cameraService.GetCamera().ScreenPointToRay(Input.mousePosition);
        }

        private void ApplyMaterialToSilhouette(Material material)
        {
            foreach (var silhouettePiece in _pieceInUse)
            {
                foreach (var renderer in silhouettePiece.renderers)
                {
                    var materials = new Material[_activeBuildable.materialCount];
                    for (int i = 0; i < _activeBuildable.materialCount; i++)
                    {
                        materials[i] = material;
                    }

                    renderer.sharedMaterials = materials;
                }
            }
        }

        private SilhouettePiece GetSilhouettePiece()
        {
            var silhouetteData = _activeBuildable.Silouette;

            if (!_pieceInPool.TryGetValue(silhouetteData.Length, out var pool))
            {
                pool = new Stack<SilhouettePiece>();
                _pieceInPool[silhouetteData.Length] = pool;
            }

            var stack = _pieceInPool[silhouetteData.Length];
            if (stack.Count > 0)
            {
                var piece = stack.Pop();

                for (var i = 0; i < silhouetteData.Length; i++)
                {
                    var data = silhouetteData[i];
                    piece.filters[i].mesh = data.GetMesh();
                }

                piece.gameObject.SetActive(true);
                _pieceInUse.Add(piece);
                return piece;
            }

            var newGameObject = new GameObject();
            newGameObject.name = silhouetteData.Length.ToString();
            var newPiece = new SilhouettePiece();
            newPiece.gameObject = newGameObject;
            newPiece.renderers = new MeshRenderer[silhouetteData.Length];
            newPiece.filters = new MeshFilter[silhouetteData.Length];

            for (var index = 0; index < silhouetteData.Length; index++)
            {
                var data = silhouetteData[index];
                var newObject = new GameObject();
                newObject.transform.SetParent(newGameObject.transform);
                var newFilter = newObject.AddComponent<MeshFilter>();

                newPiece.filters[index] = newFilter;

                newFilter.mesh = data.GetMesh();
                var newRenderer = newObject.AddComponent<MeshRenderer>();
                var materials = new Material[_activeBuildable.materialCount];

                newPiece.renderers[index] = newRenderer;

                for (int i = 0; i < _activeBuildable.materialCount; i++)
                {
                    materials[i] = _buildingData.GetAvailableMaterial();
                }

                newRenderer.sharedMaterials = materials;
            }

            _pieceInUse.Add(newPiece);

            return newPiece;
        }

        private void ReturnAllSilhouettePiecesToPool()
        {
            foreach (var silhouettePiece in _pieceInUse)
            {
                ReturnSilhouettePieceToPool(silhouettePiece);
            }

            _pieceInUse.Clear();
        }

        private void ReturnSilhouettePieceToPool(SilhouettePiece piece)
        {
            piece.gameObject.SetActive(false);
            _pieceInPool[piece.filters.Length].Push(piece);
        }

        private class SilhouettePiece
        {
            public GameObject gameObject;
            public MeshRenderer[] renderers;
            public MeshFilter[] filters;
        }
    }
}