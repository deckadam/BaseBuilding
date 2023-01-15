using System;
using System.Collections.Generic;
using Deck.Data.Damage;
using Deck.Data.General;
using Deck.Data.Item;
using Deck.EventManager;
using Deck.InputHandling.Events;
using Deck.Map;
using Deck.MVC;
using Deck.Player;
using Deck.Services;
using Deck.Services.Implementations.CameraService;
using Deck.Services.Implementations.CellSelectionService;
using Deck.Services.Implementations.GridService;
using Deck.Services.Implementations.HealthService;
using Deck.Services.Implementations.Level;
using Deck.Services.Implementations.MapService;
using Deck.Services.Implementations.Navigation;
using Deck.Services.Implementations.UIService;
using Deck.Test.General;
using Deck.Test.Markers;
using Deck.UI.Inventory;
using Deck.Utility;
using Deck.Utility.Logger;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace Deck.InputHandling
{
    public class DeckInputHandler : MonoBehaviour
    {
        public static Action<bool> OnShiftStatusChange;
        public static Action OnMouseDrag;

        [SerializeField] private DeckDamageData testDamageData;

        private DiContainer _container;
        private DeckGeneralData _generalData;
        private DeckItemData _itemData;
        private DeckCoreAgent _lastSelectedAgent;

        [Inject]
        private void Inject(DiContainer container, DeckGeneralData generalData, DeckItemData tempItem)
        {
            _container = container;
            _generalData = generalData;
            _itemData = tempItem;
        }

        private DeckGridService _deckGridService;

        private void OnEnable()
        {
            _deckGridService = DeckServiceLocator.GetService<DeckGridService>();
            DeckEventManager.Register<DeckOnCoreAgentSelected>(OnCoreAgentSelected);
        }

        private void OnDisable()
        {
            DeckEventManager.Unregister<DeckOnCoreAgentSelected>(OnCoreAgentSelected);
        }

        private void Update()
        {
            CheckForNavMeshHit();
            CheckForCoreAgentSelection(out var result);

            if (!result)
            {
                CheckForCellSelection();
            }

            RaycastToGround();
            CheckForShiftClick();
            CheckForPlayerCreation();
            CheckForTestInventoryEntry();
            CheckForInventoryUI();
            CheckForAttack();
            CheckForEscapeMenu();
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

                if (hit.transform.TryGetComponent<IDeckDamagable>(out var result))
                {
                    result.GetHealthComponent().ChangeHealth(testDamageData);
                }
            }
        }

        private void CheckForEscapeMenu()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                DeckServiceLocator.GetService<DeckUIService>().ShowWindow<DeckGamePlayUI>();
            }
        }

        private void CheckForInventoryUI()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                DeckServiceLocator.GetService<DeckUIService>().SwapStatus<DeckInventoryUI>();
            }
        }


        private void OnCoreAgentSelected(DeckOnCoreAgentSelected obj)
        {
            var inventoryController = DeckMVC<DeckItem, IEnumerable<DeckItem>>.GetController();
            _lastSelectedAgent = obj.agent;
            inventoryController.SetModel(_lastSelectedAgent.GetInventory());
        }

        private void CheckForTestInventoryEntry()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                var randomItem = _itemData.GetItems().GetRandom();
                if (_lastSelectedAgent == null) return;
                _lastSelectedAgent.GetInventory().AddData(randomItem);
                DeckLogger.Inform(randomItem.name + " add to inventory of last selected agent");
                DeckMVC<DeckItem, IEnumerable<DeckItem>>.GetController().SetModel(_lastSelectedAgent.GetInventory());
            }
        }

        private void CheckForPlayerCreation()
        {
            if (Input.GetKeyDown(KeyCode.U))
            {
                var temp = _container.InstantiatePrefab(_generalData.coreAgentPrefab, Vector3.zero, Quaternion.identity, DeckMapService.map.transform);
                var tempAgent = temp.GetComponent<DeckCoreAgent>();
                DeckServiceLocator.GetService<DeckLevelService>().AddCoreAgent(tempAgent);
            }
        }

        private void CheckForShiftClick()
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            {
                OnShiftStatusChange?.Invoke(true);
            }
            else if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
            {
                OnShiftStatusChange?.Invoke(false);
            }
        }

        private void CheckForCoreAgentSelection(out bool result)
        {
            result = false;
            if (!Input.GetMouseButtonDown(0))
            {
                return;
            }

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out var hit, 100f, 1 << 7))
            {
                return;
            }

            var agent = hit.transform.GetComponentInParent<DeckCoreAgent>();
            DeckOnCoreAgentSelected.Create(agent).Send();
            result = true;
        }

        private void CheckForCellSelection()
        {
            if (!Input.GetMouseButtonDown(0)) return;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out var hit, 1000f)) return;
            if (!hit.transform.TryGetComponent<DeckGamePlayMap>(out var result)) return;
            var pos = hit.point;
            var cell = _deckGridService.GetCellWithWorldPosition(pos);
            if (cell == null) return;
            DeckOnCellClicked.Create(cell).Send();
        }

        private void OnDrawGizmos()
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        private void CheckForNavMeshHit()
        {
            if (!Input.GetMouseButton(1)) return;
            var screenPosition = Input.mousePosition;
            var ray = Camera.main.ScreenPointToRay(screenPosition);
            var positionOnGroundPlane = ray.origin - ray.direction / ray.direction.y * ray.origin.y; //collide with plane at y=0
            if (!NavMesh.SamplePosition(positionOnGroundPlane, out var navMeshHit, 1, 1)) return;

            DeckOnNavMeshPositionSelection.Create(navMeshHit.position).Send();
        }

        private void RaycastToGround()
        {
            var screenPosition = Input.mousePosition;
            var ray = Camera.main.ScreenPointToRay(screenPosition);
            if (!Physics.Raycast(ray, out var hit, 100f, 1 << 6)) return;
            var pos = hit.textureCoord;
            DeckOnGroundPositionChange.Create(pos).Send();
        }
    }
}