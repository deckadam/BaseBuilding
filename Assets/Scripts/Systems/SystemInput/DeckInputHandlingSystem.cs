using Deck.Components;
using Deck.Events;
using Deck.Events.CameraService;
using Deck.Events.CellSelectionService;
using Deck.Events.MapService;
using Deck.Data.Damage;
using Deck.Data.Item;
using Deck.InputHandling.Events;
using Deck.Item;
using Deck.Save;
using Deck.Services.Building;
using Deck.UI;
using Deck.UI.GamePlay;
using Deck.UI.Inventory;
using Deck.Utility.Class;
using Deck.Utility.Logger;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace Deck.InputHandling
{
    public class DeckInputHandlingSystem : MonoBehaviour
    {
        [SerializeField] private DeckDataDamage testDataDamage;
        private float _lastPressTime;
        private DeckBinderItem _binderItem;
        private DeckServiceSelection _serviceSelection;
        private DeckAgentCore _coreAgentPrefab;
        private DiContainer _container;

        [Inject]
        private void Inject(DiContainer container, DeckBinderItem tempBinderItem, DeckAgentCore coreAgentPrefab)
        {
            _container = container;
            _coreAgentPrefab = coreAgentPrefab;
            _binderItem = tempBinderItem;
        }

        private void Awake()
        {
            _serviceSelection = Deck.GetService<DeckServiceSelection>();
        }

        private void Update()
        {
            CheckForNavMeshHit();
            CheckForSelectable();
            RaycastToGround();
            CheckForPlayerCreation();
            CheckForTestInventoryEntry();
            OpenInventoryPopUpForSelectable();
            CheckForAttack();
            CheckForEscapeMenu();
            SaveCheck();
            CheckForBuilding();
            CheckForClickable();
        }

        private void CheckForClickable()
        {
            if (Input.GetMouseButtonDown(1))
            {
                var ray = Deck.GetService<DeckServiceCamera>().GetCamera().ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hit, 250f))
                {
                    if (hit.transform.TryGetComponent<DeckItemVisual>(out var result))
                    {
                        result.PickUp();
                    }
                }
            }
        }

        private void CheckForBuilding()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                var buildService = Deck.GetService<DeckServiceBuilding>();
                var buildData = buildService.GetBuildable("Chest");
                buildService.StartSilouette(buildData);
            }

            if (Input.GetKey(KeyCode.B))
            {
                Deck.GetService<DeckServiceBuilding>().UpdateSilouette(GetCellPosition(GetWorldPosition()));
            }

            if (Input.GetKeyUp(KeyCode.B))
            {
                var buildService = Deck.GetService<DeckServiceBuilding>();
                var buildData = buildService.GetBuildable("Chest");
                var cellPosition = GetCellPosition(GetWorldPosition());

                buildService.StopSilouette();
                if (!buildService.CheckIfAgentBuildableInArea(buildData, cellPosition))
                {
                    DeckNotificationRequestedEvent.Create(DeckConstantsNotification.OnBuildingAreaIsNotClear).Send();
                    return;
                }

                var hasItems = DeckServiceSelection.currentPossession.GetDeckComponent<DeckComponentInventory>().ReduceIfPossible(buildData.GetMaterials());
                if (!hasItems)
                {
                    DeckNotificationRequestedEvent.Create(DeckConstantsNotification.OnItemRequirementNotMet).Send();
                    return;
                }

                var newChest = buildService.Build<DeckAgentChest>(buildData);
                newChest.transform.position = cellPosition;
                newChest.StartWithClearData();
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                var buildService = Deck.GetService<DeckServiceBuilding>();
                var buildData = buildService.GetBuildable("WoodAndStoneWall");
                buildService.StartSilouette(buildData);
            }

            if (Input.GetKey(KeyCode.C))
            {
                Deck.GetService<DeckServiceBuilding>().UpdateSilouette(GetCellPosition(GetWorldPosition()));
            }

            if (Input.GetKeyUp(KeyCode.C))
            {
                var buildService = Deck.GetService<DeckServiceBuilding>();
                var buildData = buildService.GetBuildable("WoodAndStoneWall");
                var cellPosition = GetCellPosition(GetWorldPosition());

                buildService.StopSilouette();
                if (!buildService.CheckIfAgentBuildableInArea(buildData, cellPosition))
                {
                    DeckNotificationRequestedEvent.Create(DeckConstantsNotification.OnBuildingAreaIsNotClear).Send();
                    return;
                }

                var hasItems = DeckServiceSelection.currentPossession.GetDeckComponent<DeckComponentInventory>().ReduceIfPossible(buildData.GetMaterials());
                if (!hasItems)
                {
                    DeckNotificationRequestedEvent.Create(DeckConstantsNotification.OnItemRequirementNotMet).Send();
                    return;
                }

                var newChest = buildService.Build<DeckAgent>(buildData);
                newChest.transform.parent = DeckServiceMap.GetMap().transform;
                newChest.transform.position = cellPosition;
                newChest.StartWithClearData();
            }
        }

        private Vector3 GetCellPosition(Vector3 worldPosition)
        {
            var x = Mathf.RoundToInt(worldPosition.x);
            var y = Mathf.RoundToInt(worldPosition.y);
            var z = Mathf.RoundToInt(worldPosition.z);

            return new Vector3(x, y, z);
        }


        private Vector3 GetWorldPosition()
        {
            var screenPosition = Input.mousePosition;
            var ray = Deck.GetService<DeckServiceCamera>().GetCamera().ScreenPointToRay(screenPosition);
            var positionOnGroundPlane = ray.origin - ray.direction / ray.direction.y * ray.origin.y; //collide with plane at y=0

            if (!NavMesh.SamplePosition(positionOnGroundPlane, out var navMeshHit, 1, 1))
            {
                return Vector3.zero;
            }

            return navMeshHit.position;
        }

        private void SaveCheck()
        {
            if (Input.GetKeyDown(KeyCode.F5))
            {
                Deck.GetService<DeckGameManager>().Test_FillSaveFile();
                DeckSaveSystem.Save();
            }
        }

        private void CheckForAttack()
        {
            if (!Input.GetMouseButtonDown(1))
            {
                return;
            }

            if (DeckServiceSelection.currentPossession == null)
            {
                return;
            }

            var ray = Deck.GetService<DeckServiceCamera>().GetCamera().ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out var hit, 100f))
            {
                return;
            }

            if (hit.transform.TryGetComponent<DeckAgent>(out var result))
            {
                var damageDealer = DeckServiceSelection.currentPossession.GetDeckComponent<DeckComponentDamageDealer>();
                damageDealer?.DealDamage(result);
            }
        }

        private void CheckForEscapeMenu()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Deck.GetService<DeckServiceUI>().ShowWindow<DeckGamePlayUI>();
            }
        }

        private void OpenInventoryPopUpForSelectable()
        {
            if (DeckServiceSelection.currentSelection != null && Input.GetKeyDown(KeyCode.I))
            {
                var newPopUp = Deck.GetService<DeckServicePopUp>().GetPopUp<DeckInventoryPopUp, DeckInventoryPopUp.Factory>().Create();
                var inventoryComponent = DeckServiceSelection.currentSelection.GetDeckComponent<DeckComponentInventory>();
                if (inventoryComponent != null)
                {
                }

                newPopUp.SetTarget(inventoryComponent);
            }
        }

        private void CheckForTestInventoryEntry()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                if (DeckServiceSelection.currentSelection == null) return;
                var randomItem = _binderItem.GetItems().GetRandom();
                var itemInstance = Instantiate(randomItem);
                DeckServiceSelection.currentSelection.AddCommand(new DeckCommandAddItem(itemInstance, DeckServiceSelection.currentSelection.GetDeckComponent<DeckComponentInventory>()));
                DeckLogger.Inform(randomItem.name + " add to inventory of last selected agent");
            }
        }

        private void CheckForPlayerCreation()
        {
            if (Input.GetKeyDown(KeyCode.U))
            {
                var newAgent = _container.InstantiatePrefab(_coreAgentPrefab);
                newAgent.GetComponent<DeckAgent>().StartWithClearData();
            }
        }

        private void CheckForSelectable()
        {
            if (!Input.GetMouseButtonDown(0))
            {
                return;
            }

            var ray = Deck.GetService<DeckServiceCamera>().GetCamera().ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out var hit, 100f))
            {
                return;
            }

            if (!hit.transform.TryGetComponent<DeckAgent>(out var componentHolder))
            {
                return;
            }

            var currentPressTime = Time.time;
            var delta = currentPressTime - _lastPressTime;
            _lastPressTime = currentPressTime;

            var canPossess = delta < 0.15f;

            if (canPossess)
            {
                Deck.GetService<DeckServiceSelection>().OnPossession(componentHolder);
            }
            else
            {
                Deck.GetService<DeckServiceSelection>().OnSelection(componentHolder);
            }
        }

        private void CheckForNavMeshHit()
        {
            if (!Input.GetMouseButton(1))
            {
                return;
            }

            if (DeckServiceSelection.currentPossession == null)
            {
                return;
            }

            var screenPosition = Input.mousePosition;
            var ray = Deck.GetService<DeckServiceCamera>().GetCamera().ScreenPointToRay(screenPosition);
            var positionOnGroundPlane = ray.origin - ray.direction / ray.direction.y * ray.origin.y; //collide with plane at y=0
            if (!NavMesh.SamplePosition(positionOnGroundPlane, out var navMeshHit, 1, 1)) return;

            new DeckCommandMove(navMeshHit.position, DeckServiceSelection.currentPossession).ProcessCommand(default);
        }

        private void RaycastToGround()
        {
            var screenPosition = Input.mousePosition;
            var ray = Deck.GetService<DeckServiceCamera>().GetCamera().ScreenPointToRay(screenPosition);
            if (!Physics.Raycast(ray, out var hit, 100f, 1 << 6)) return;
            var pos = hit.textureCoord;
            DeckOnGroundPositionChangeEvent.Create(pos).Send();
        }
    }
}