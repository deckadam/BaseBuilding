using Deck.Agent;
using Deck.Components;
using Deck.Data.Damage;
using Deck.Data.Item;
using Deck.GameManager;
using Deck.InputHandling.Events;
using Deck.Map.Agent.Chest;
using Deck.Save;
using Deck.Services.Building;
using Deck.Services.Implementations;
using Deck.Services.Implementations.CellSelectionService;
using Deck.Services.Implementations.GridService;
using Deck.Utility.Constants.GamePlay;
using Deck.Utility.Constants.Inventory;
using Deck.Utility;
using Deck.Utility.Logger;
using UnityEngine;
using UnityEngine.AI;
using Deck.Utility.Constants;
using Zenject;

namespace Deck.InputHandling
{
    public class DeckInputHandlingSystem : MonoBehaviour
    {
        [SerializeField] private DeckDataDamage testDataDamage;
        private float _lastPressTime;
        private DeckBinderItem _binderItem;
        private DeckAgentCore.Factory _coreAgentFactory;
        private Camera _camera;
        private DeckSelectionService _selectionService;

        [Inject]
        private void Inject(DiContainer container, DeckBinderItem tempBinderItem, DeckAgentCore.Factory agentFactory, Camera camera)
        {
            _binderItem = tempBinderItem;
            _coreAgentFactory = agentFactory;
            _camera = camera;
        }

        private void Awake()
        {
            _selectionService = Deck.GetService<DeckSelectionService>();
        }

        private void Update()
        {
            CheckForNavMeshHit();
            CheckForSelectable(out var result);

            if (!result)
            {
                CheckForCellSelection();
            }

            RaycastToGround();
            CheckForPlayerCreation();
            CheckForTestInventoryEntry();
            OpenInventoryPopUpForSelectable();
            CheckForAttack();
            CheckForEscapeMenu();
            SaveCheck();
            CheckForBuilding();
        }

        private void CheckForBuilding()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                var buildService = Deck.GetService<DeckBuildingService>();
                var buildData = buildService.GetBuildable("Chest");

                var hasItems = DeckSelectionService.currentPossession.GetDeckComponent<DeckComponentInventory>().ReduceIfPossible(buildData.GetMaterials());
                if (!hasItems)
                {
                    DeckNotificationRequestedEvent.Create(DeckConstantsNotification.OnItemRequirementNotMet).Send();
                    return;
                }

                var newChest = buildService.Build<DeckAgentChest>(buildData);
                newChest.transform.position = GetWorldPosition();
                newChest.StartWithClearData();
            }
        }

        private Vector3 GetWorldPosition()
        {
            var screenPosition = Input.mousePosition;
            var ray = Camera.main.ScreenPointToRay(screenPosition);
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
            if (Input.GetMouseButtonDown(1))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (!Physics.Raycast(ray, out var hit, 100f))
                {
                    return;
                }

                if (hit.transform.TryGetComponent<DeckAgent>(out var result))
                {
                    new DeckCommandDamage(testDataDamage, result).ProcessCommand();
                }
            }
        }

        private void CheckForEscapeMenu()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Deck.GetService<DeckUIService>().ShowWindow<DeckGamePlayUI>();
            }
        }

        private void OpenInventoryPopUpForSelectable()
        {
            if (DeckSelectionService.currentSelection != null && Input.GetKeyDown(KeyCode.I))
            {
                var newPopUp = Deck.GetService<DeckPopUpService>().GetPopUp<DeckInventoryPopUp, DeckInventoryPopUp.Factory>().Create();
                var inventoryComponent = DeckSelectionService.currentSelection.GetDeckComponent<DeckComponentInventory>();
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
                if (DeckSelectionService.currentSelection == null) return;
                var randomItem = _binderItem.GetItems().GetRandom();
                var itemInstance = Instantiate(randomItem);
                DeckSelectionService.currentSelection.AddCommand(new DeckCommandAddItem(itemInstance, DeckSelectionService.currentSelection.GetDeckComponent<DeckComponentInventory>()));
                DeckLogger.Inform(randomItem.name + " add to inventory of last selected agent");
            }
        }

        private void CheckForPlayerCreation()
        {
            if (Input.GetKeyDown(KeyCode.U))
            {
                _coreAgentFactory.Create().StartWithClearData();
            }
        }

        private void CheckForSelectable(out bool result)
        {
            result = false;
            if (!Input.GetMouseButtonDown(0))
            {
                return;
            }

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
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
                Deck.GetService<DeckSelectionService>().OnPossession(componentHolder);
            }
            else
            {
                Deck.GetService<DeckSelectionService>().OnSelection(componentHolder);
            }

            result = true;
        }

        private void CheckForCellSelection()
        {
            if (!Input.GetMouseButtonDown(0)) return;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out var hit, 1000f)) return;
            if (!hit.transform.CompareTag(DeckConstantsTag.MAP)) return;
            var pos = hit.point;
            var cell = Deck.GetService<DeckGridService>().GetCellWithWorldPosition(pos);
            if (cell == null) return;
            DeckOnCellClickedEvent.Create(cell).Send();
        }

        private void CheckForNavMeshHit()
        {
            if (!Input.GetMouseButton(1)) return;
            var screenPosition = Input.mousePosition;
            var ray = Camera.main.ScreenPointToRay(screenPosition);
            var positionOnGroundPlane = ray.origin - ray.direction / ray.direction.y * ray.origin.y; //collide with plane at y=0
            if (!NavMesh.SamplePosition(positionOnGroundPlane, out var navMeshHit, 1, 1)) return;

            new DeckCommandMove(navMeshHit.position, DeckSelectionService.currentPossession).ProcessCommand();
        }

        private void RaycastToGround()
        {
            var screenPosition = Input.mousePosition;
            var ray = Camera.main.ScreenPointToRay(screenPosition);
            if (!Physics.Raycast(ray, out var hit, 100f, 1 << 6)) return;
            var pos = hit.textureCoord;
            DeckOnGroundPositionChangeEvent.Create(pos).Send();
        }
    }
}