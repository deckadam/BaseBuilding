using Base;
using Data.Currency;
using Deck.Components;
using Deck.Services.Cam;
using Deck.Services.PopUp;
using Deck.Services.Selection;
using Deck.Utility.MonoBehaviours;
using GameManager;
using InGame.Agent.Customer;
using Instancing;
using Services;
using Services.Currency;
using Services.Escapable;
using Systems.SystemInput.Events;
using Systems.SystemSave;
using UI.Inventory;
using UnityEngine;
using UnityEngine.EventSystems;
using Utility;
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
        }

        private void CheckForSpawnCustomer()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                var newCustomer = _instanceProvider.RentAgent(_customerAgentPrefab.PrefabId);
                newCustomer.transform.position = new Vector3(20, 0, 20);
                newCustomer.Initialize();
            }
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
            if (!Input.GetKeyDown(KeyCode.Delete)) return;

            var ray = DeckServiceProvider.GetService<DeckServiceCamera>().GetRayFromCamera();
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