using Deck.Commands;
using Deck.Components;
using Deck.Components.Core;
using Deck.Data.Currency;
using Deck.Data.Item;
using Deck.GameManager;
using Deck.InputHandling.Events;
using Deck.Save;
using Deck.Services;
using Deck.Services.CameraService;
using Deck.Services.CellSelectionService;
using Deck.Services.Implementations.Currency;
using Deck.Services.Implementations.Escapable;
using Deck.UI.Inventory;
using Deck.Utility;
using Deck.Utility.MonoBehaviours;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using Zenject;

namespace Deck.InputHandling
{
    public class DeckInputHandlingSystem : MonoBehaviour
    {
        public static bool IsInterruptingCommandModeActive;
        private float _lastPressTime;
        private DeckBinderItem _binderItem;
        private DeckAgentCore _coreAgentPrefab;
        private DiContainer _container;

        [Inject]
        private void Inject(DiContainer container, DeckBinderItem tempBinderItem, DeckAgentCore coreAgentPrefab)
        {
            _container = container;
            _coreAgentPrefab = coreAgentPrefab;
            _binderItem = tempBinderItem;
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
            CheckForCommandStackStatus();
            CheckForStatsPopUp();
            CheckForDestroyBuilding();
            CheckForCurrency();
        }

        private void CheckForCurrency()
        {
            if (Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                Deck.GetService<DeckServiceCurrency>().ChangeValueRelative(DeckCurrencyType.Money, 50);
            }
            else if (Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                Deck.GetService<DeckServiceCurrency>().ChangeValueRelative(DeckCurrencyType.Money, -50);
            }
        }

        private void CheckForDestroyBuilding()
        {
            if (!Input.GetKeyDown(KeyCode.Delete)) return;

            var ray = Deck.GetService<DeckServiceCamera>().GetRayFromCamera();
            if (!Physics.Raycast(ray, out var hit)) return;

            if (!hit.transform.TryGetComponentInParent<DeckAgent>(out var agent)) return;

            agent.RequestDestroy();
        }

        private void CheckForStatsPopUp()
        {
            // if (Input.GetKeyDown(KeyCode.C))
            // {
            // var newPopUp = Deck.GetService<DeckServicePopUp>().OpenPopUp<DeckStatsPopUp>();
            // newPopUp.Show();
            // newPopUp.ShowStats(DeckServiceSelection.currentSelection);
            // }
        }

        private void CheckForCommandStackStatus()
        {
            IsInterruptingCommandModeActive = !Input.GetKey(KeyCode.LeftShift);
        }

        private void SaveCheck()
        {
            if (Input.GetKeyDown(KeyCode.F5))
            {
                Deck.GetService<DeckGameManager>().GatherSaveData();
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

            if (hit.transform.TryGetComponentInParent<DeckAgent>(out var result))
            {
                var damageDealer = DeckServiceSelection.currentPossession.GetDeckComponent<DeckComponentCommandCreator>();
                if (damageDealer)
                {
                    damageDealer.DealDamage(result);
                }
            }
        }

        private void CheckForEscapeMenu()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Deck.GetService<DeckServiceEscapable>().CloseEscapable();
            }
        }

        private void OpenInventoryPopUpForSelectable()
        {
            if (DeckServiceSelection.currentSelection != null && Input.GetKeyDown(KeyCode.I))
            {
                var newPopUp = Deck.GetService<DeckServicePopUp>().OpenPopUp<DeckInventoryPopUp>();
                var inventoryComponent = DeckServiceSelection.currentSelection.GetDeckComponent<DeckComponentInventory>();
                if (inventoryComponent == null)
                {
                    return;
                }

                newPopUp.Show();
                newPopUp.SetTarget(inventoryComponent);
            }
        }

        private void CheckForTestInventoryEntry()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                if (DeckServiceSelection.currentSelection == null) return;
                var items = _binderItem.GetItems();
                foreach (var deckDataItem in items)
                {
                    DeckServiceSelection.currentSelection.EnqueueCommand(new DeckCommandAddItem(deckDataItem, 100, DeckServiceSelection.currentSelection.GetDeckComponent<DeckComponentInventory>()));
                    DeckLogger.Inform(deckDataItem.name + " add to inventory of last selected agent");
                }
            }
        }

        private void CheckForPlayerCreation()
        {
            if (Input.GetKeyDown(KeyCode.U))
            {
                var newAgent = _container.InstantiatePrefab(_coreAgentPrefab);
                newAgent.GetComponent<DeckAgent>().Initialize();
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

            var componentHolder = hit.transform.GetComponentInParent<DeckAgent>();
            if (componentHolder == null)
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
            if (!Input.GetMouseButtonDown(1))
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
            if (!NavMesh.SamplePosition(positionOnGroundPlane, out var navMeshHit, 100, 1))
            {
                return;
            }

            DeckServiceSelection.currentPossession.AddCommand(new DeckCommandMove(navMeshHit.position, DeckServiceSelection.currentPossession));
        }

        private bool _isInputStartedOnUI;
        private Vector2Int _lastInputPosition;

        private void RaycastToGround()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (IsOnUI())
                {
                    _isInputStartedOnUI = true;
                }
                else
                {
                    _isInputStartedOnUI = false;
                    _lastInputPosition = Input.mousePosition.ToVector2Int();
                    DeckEventOnLeftClickDown.Create(Deck.GetService<DeckServiceCamera>().GetCursorWorldPosition()).Send();
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (_isInputStartedOnUI)
                {
                    return;
                }

                DeckEventOnLeftClickUp.Create(Deck.GetService<DeckServiceCamera>().GetCursorWorldPosition(), IsOnUI()).Send();
            }

            if (Input.mouseScrollDelta.y != 0)
            {
                DeckEventMiddleScroll.Create(Input.mouseScrollDelta.y).Send();
            }

            var currentInputPosition = Input.mousePosition.ToVector2Int();

            if (_lastInputPosition == currentInputPosition)
            {
                return;
            }

            _lastInputPosition = currentInputPosition;
            DeckEventOnMouseMove.Create(Deck.GetService<DeckServiceCamera>().GetCursorWorldPosition()).Send();
        }

        private bool IsOnUI()
        {
            return EventSystem.current.IsPointerOverGameObject();
        }
    }
}