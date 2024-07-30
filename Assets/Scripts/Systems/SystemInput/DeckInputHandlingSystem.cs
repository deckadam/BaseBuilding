using Deck.Agent;
using Deck.Commands;
using Deck.Data.Damage;
using Deck.Data.Item;
using Deck.InputHandling.Events;
using Deck.Save;
using Deck.Services;
using Deck.Services.Building;
using Deck.Services.CameraService;
using Deck.Services.CellSelectionService;
using Deck.UI;
using Deck.UI.Inventory;
using Deck.UI.Stats;
using Deck.Utility.Logger;
using Deck.Utility.MonoBehaviours;
using Services.Implementations.Escapable;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace Deck.InputHandling
{
    public class DeckInputHandlingSystem : MonoBehaviour
    {
        [SerializeField] private DeckDataDamage testDataDamage;
        public static bool IsInterruptingCommandModeActive;
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
            CheckForCommandStackStatus();
            CheckForStatsPopUp();
        }

        private void CheckForStatsPopUp()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                var newPopUp = Deck.GetService<DeckServicePopUp>().OpenPopUp<DeckStatsPopUp>();
                newPopUp.Show();
                newPopUp.ShowStats(DeckServiceSelection.currentSelection);
            }
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
                    DeckServiceSelection.currentSelection.EnqueCommand(new DeckCommandAddItem(deckDataItem, 100, DeckServiceSelection.currentSelection.GetDeckComponent<DeckComponentInventory>()));
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

        private void RaycastToGround()
        {
            if (Input.GetMouseButtonDown(0))
            {
                DeckEventOnLeftClick.Create(Deck.GetService<DeckServiceCamera>().GetCursorWorldPosition()).Send();
            }
        }
    }
}