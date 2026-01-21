using Base;
using Components.Inventory;
using Data.Currency;
using GameManager;
using InGame.Agent.Customer;
using Instancing;
using Services;
using Services.Camera;
using Services.Currency;
using Services.Escapable;
using Services.ItemVisual;
using Services.PopUp;
using Services.Selection;
using Systems.SystemInput.Events;
using Systems.SystemSave;
using UI.Inventory;
using UI.Stats;
using UnityEngine;
using UnityEngine.EventSystems;
using Utility;
using Utility.MonoBehaviours;
using Zenject;

namespace Systems.SystemInput
{
    public class DeckInputHandlingSystem : MonoBehaviour
    {
        private float _lastPressTime;
        private DeckInstanceProvider _instanceProvider;

        [SerializeField] private DeckAgentCustomer _customerAgentPrefab;

        [Inject]
        private void Inject(DeckInstanceProvider instanceProvider)
        {
            _instanceProvider = instanceProvider;
        }

        private void Update()
        {
            CheckForSelectable();
            RaycastToGround();
            OpenInventoryPopUpForSelectable();
            CheckForEscapeMenu();
            SaveCheck();
            CheckForStatsPopUp();
            CheckForDestroyBuilding();
            CheckForCurrency();
            CheckForSpawnCustomer();
            CheckForCameraMovement();
        }

        private void CheckForCameraMovement()
        {
            var horizontal = Input.GetAxis("Horizontal");
            var vertical = Input.GetAxis("Vertical");
            DeckEventOnAxisMovement.Create(new Vector2(horizontal, vertical)).Send();
        }

        private void CheckForSpawnCustomer()
        {
            if (!Input.GetKeyDown(KeyCode.T)) return;

            var newCustomer = _instanceProvider.RentAgent(_customerAgentPrefab.PrefabId);
            newCustomer.transform.position = new Vector3(20, 0, 20);
            newCustomer.Initialize();
        }

        private void CheckForCurrency()
        {
            if (Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                DeckServiceProvider.GetService<DeckServiceCurrency>().ChangeValueRelative(DeckCurrencyType.Money, 50);
            }
            else if (Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                DeckServiceProvider.GetService<DeckServiceCurrency>().ChangeValueRelative(DeckCurrencyType.Money, -50);
            }
        }

        private void CheckForDestroyBuilding()
        {
            if (!Input.GetKey(KeyCode.Delete)) return;

            var ray = DeckServiceProvider.GetService<DeckServiceCamera>().GetRayFromCamera();
            if (!Physics.Raycast(ray, out var hit)) return;

            if (hit.transform.TryGetComponentInParent<DeckAgent>(out var agent))
            {
                agent.RequestDestroy();
            }
            else if (hit.transform.TryGetComponentInParent<DeckItemVisual>(out var itemVisual))
            {
                DeckServiceProvider.GetService<DeckServiceItemVisual>().ReturnItemVisual(itemVisual);
            }
        }

        private void CheckForStatsPopUp()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                var newPopUp = DeckServiceProvider.GetService<DeckServicePopUp>().OpenPopUp<DeckStatsPopUp>();
                newPopUp.Show();
                newPopUp.ShowStats(DeckServiceSelection.currentSelection);
            }
        }

        private void SaveCheck()
        {
            if (Input.GetKeyDown(KeyCode.F5))
            {
                DeckServiceProvider.GetService<DeckGameManager>().GatherSaveData();
                DeckSaveSystem.Save();
            }
        }

        private void CheckForEscapeMenu()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                DeckServiceProvider.GetService<DeckServiceEscapable>().CloseEscapable();
            }
        }

        private void OpenInventoryPopUpForSelectable()
        {
            if (DeckServiceSelection.currentSelection != null && Input.GetKeyDown(KeyCode.I))
            {
                var newPopUp = DeckServiceProvider.GetService<DeckServicePopUp>().OpenPopUp<DeckInventoryPopUp>();
                var inventoryComponent = DeckServiceSelection.currentSelection.GetDeckComponent<DeckComponentInventory>();
                if (inventoryComponent == null)
                {
                    return;
                }

                newPopUp.Show();
                newPopUp.SetTarget(inventoryComponent);
            }
        }


        private void CheckForSelectable()
        {
            if (!Input.GetMouseButtonDown(0))
            {
                return;
            }

            var ray = DeckServiceProvider.GetService<DeckServiceCamera>().GetCamera().ScreenPointToRay(Input.mousePosition);
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
                DeckServiceProvider.GetService<DeckServiceSelection>().OnPossession(componentHolder);
            }
            else
            {
                DeckServiceProvider.GetService<DeckServiceSelection>().OnSelection(componentHolder);
            }
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
                    DeckEventOnLeftClickDown.Create(DeckServiceProvider.GetService<DeckServiceCamera>().GetCursorWorldPosition()).Send();
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (_isInputStartedOnUI)
                {
                    return;
                }

                DeckEventOnLeftClickUp.Create(DeckServiceProvider.GetService<DeckServiceCamera>().GetCursorWorldPosition(), IsOnUI()).Send();
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
            DeckEventOnMouseMove.Create(DeckServiceProvider.GetService<DeckServiceCamera>().GetCursorWorldPosition()).Send();
        }

        private bool IsOnUI()
        {
            return EventSystem.current.IsPointerOverGameObject();
        }
    }
}