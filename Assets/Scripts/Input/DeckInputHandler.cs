using System;
using Deck.Data.Damage;
using Deck.Data.General;
using Deck.Data.Item;
using Deck.InputHandling.Events;
using Deck.Map;
using Deck.Player;
using Deck.SaveService;
using Deck.Services;
using Deck.Services.Implementations;
using Deck.Services.Implementations.GridService;
using Deck.Services.Implementations.Level;
using Deck.Services.Implementations.MapService;
using Deck.Test;
using Deck.Test.General;
using Deck.Test.Markers;
using Deck.UI.GamePlay;
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
        private IDeckSelectable _lastSelection;

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
            CheckForShiftClick();
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
                DeckServiceLocator.GetService<DeckGameManager>().Test_FillSaveFile();
                DeckSaveManager.Save();
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

        private void OpenInventoryPopUpForSelectable()
        {
            if (_lastSelection != null && Input.GetKeyDown(KeyCode.I))
            {
                var newPopUp = DeckServiceLocator.GetService<DeckPopUpService>().GetPopUp<DeckInventoryPopUp, DeckInventoryPopUp.Factory>().Create();
                newPopUp.SetTarget(_lastSelection.GetInventory());
            }
        }

        private void CheckForTestInventoryEntry()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                if (_lastSelection == null) return;
                var randomItem = _itemData.GetItems().GetRandom();
                var itemInstance = Instantiate(randomItem);
                _lastSelection.GetInventory().AddItem(itemInstance);
                DeckLogger.Inform(randomItem.name + " add to inventory of last selected agent");
            }
        }

        private void CheckForPlayerCreation()
        {
            if (Input.GetKeyDown(KeyCode.U))
            {
                var temp = _container.InstantiatePrefab(_generalData.coreAgentPrefab, Vector3.zero, Quaternion.identity, DeckMapService.map.transform);
                var tempAgent = temp.GetComponent<DeckCoreAgent>();
                tempAgent.LoadData(Guid.NewGuid().ToString());
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

            if (!hit.transform.TryGetComponent<IDeckSelectable>(out var selectable))
            {
                return;
            }

            _lastSelection?.OnPossesFinished();

            _lastSelection = selectable;

            _lastSelection.OnPossesStarted();
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
            DeckOnCellClickedEvent.Create(cell).Send();
        }

        private void CheckForNavMeshHit()
        {
            if (!Input.GetMouseButton(1)) return;
            var screenPosition = Input.mousePosition;
            var ray = Camera.main.ScreenPointToRay(screenPosition);
            var positionOnGroundPlane = ray.origin - ray.direction / ray.direction.y * ray.origin.y; //collide with plane at y=0
            if (!NavMesh.SamplePosition(positionOnGroundPlane, out var navMeshHit, 1, 1)) return;

            DeckOnNavMeshPositionSelectionEvent.Create(navMeshHit.position).Send();
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