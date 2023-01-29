using System;
using Deck.Agent;
using Deck.Component;
using Deck.Components;
using Deck.Data.Damage;
using Deck.Data.Item;
using Deck.GameManager;
using Deck.InputHandling.Events;
using Deck.Inventory;
using Deck.Map;
using Deck.Save;
using Deck.Services.Implementations;
using Deck.Services.Implementations.CellSelectionService;
using Deck.Services.Implementations.GridService;
using Deck.UI.GamePlay;
using Deck.UI.Inventory;
using Deck.Utility;
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
        private DeckCoreAgent.Factory _agentFactory;
        private Camera _camera;
        private DeckSelectionService _selectionService;

        [Inject]
        private void Inject(DiContainer container, DeckBinderItem tempBinderItem, DeckCoreAgent.Factory agentFactory, Camera camera)
        {
            _binderItem = tempBinderItem;
            _agentFactory = agentFactory;
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
                    result.DispatchCommand(new DeckCommandDamage(testDataDamage));
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
                newPopUp.SetTarget(DeckSelectionService.currentSelection.GetDeckComponent<DeckInventoryComponent>());
            }
        }

        private void CheckForTestInventoryEntry()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                if (DeckSelectionService.currentSelection == null) return;
                var randomItem = _binderItem.GetItems().GetRandom();
                var itemInstance = Instantiate(randomItem);
                DeckSelectionService.currentSelection.DispatchCommand(new DeckCommandAddItem(itemInstance));
                DeckLogger.Inform(randomItem.name + " add to inventory of last selected agent");
            }
        }

        private void CheckForPlayerCreation()
        {
            if (Input.GetKeyDown(KeyCode.U))
            {
                _agentFactory.Create().LoadData(Guid.NewGuid().ToString());
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
            if (!hit.transform.TryGetComponent<DeckMap>(out var result)) return;
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

            _selectionService.GetAgentToBeCommanded().DispatchCommand(new DeckCommandMove(navMeshHit.position));
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