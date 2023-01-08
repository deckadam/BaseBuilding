using Deck.Data.Gizmo;
using Deck.Data.Item;
using Deck.EventManager;
using Deck.InputHandling.Events;
using Deck.Inventory.Item;
using Deck.Inventory.UI;
using Deck.Player;
using Deck.Services;
using Deck.Services.Implementations.CameraService;
using Deck.Services.Implementations.CellSelectionService;
using Deck.Services.Implementations.GridService;
using Deck.Services.Implementations.Level;
using Deck.Services.Implementations.MapService;
using Deck.Services.Implementations.Navigation;
using Deck.Services.Implementations.UIService;
using Deck.Test.General;
using Deck.Utility;
using Deck.Utility.Logger;
using MVC.DeckMVCUIController;
using UnityEngine;
using Zenject;

namespace Deck.Test
{
    public class DeckGameTest : MonoBehaviour
    {
        [Inject] private DiContainer container;
        [Inject] private DeckGizmoData gizmoData;
        [Inject] private DeckCoreAgent deckCoreAgent;
        [Inject] private DeckItemData itemData;
        [Inject] private DeckInventoryDisplayer inventoryDisplayer;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void InitializeGame()
        {
            DeckMVC<DeckItem>.ResetController();
        }

        private DeckCoreAgent _lastSelectedAgent;

        private void OnEnable()
        {
            DeckEventManager.Register<DeckOnCoreAgentSelected>(OnCoreAgentSelected);
        }

        private void OnDisable()
        {
            DeckEventManager.Unregister<DeckOnCoreAgentSelected>(OnCoreAgentSelected);
        }

        private void OnCoreAgentSelected(DeckOnCoreAgentSelected obj)
        {
            var inventoryController = DeckMVC<DeckItem>.GetController();
            if (_lastSelectedAgent != null)
            {
                inventoryController.ResetModel();
            }

            _lastSelectedAgent = obj.agent;
            inventoryController.SetModel(_lastSelectedAgent.GetInventory());
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                DeckServiceLocator.GetService<DeckMapService>().CreateMap();

                DeckServiceLocator.GetService<DeckGridService>().GenerateGrid(out var grid);
                DeckServiceLocator.GetService<DeckMapService>().CreateGround(grid, out var ground, out var groundMaterial);
                DeckServiceLocator.GetService<DeckMapService>().PopulateMap(grid);
                DeckServiceLocator.GetService<DeckNavigationService>().GenerateNavigation(out var surface);
                DeckServiceLocator.GetService<DeckCameraService>().GenerateCameraBounds(grid);
                DeckServiceLocator.GetService<DeckLevelService>().SetGrid(grid);
                DeckServiceLocator.GetService<DeckCellSelectionService>().SetMapData(grid.size, groundMaterial);
                DeckServiceLocator.GetService<DeckMapService>().InitializeMap(grid, surface, ground);
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                var temp = container.InstantiatePrefab(deckCoreAgent, Vector3.zero, Quaternion.identity, DeckMapService.map.transform);
                var tempAgent = temp.GetComponent<DeckCoreAgent>();
                DeckServiceLocator.GetService<DeckLevelService>().AddCoreAgent(tempAgent);
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                var randomItem = itemData.items.GetRandom();
                if (_lastSelectedAgent == null) return;
                _lastSelectedAgent.GetInventory().AddItem(randomItem);
                DeckLogger.Inform(randomItem.name + " add to inventory of last selected agent");
                DeckMVC<DeckItem>.GetController().SetModel(_lastSelectedAgent.GetInventory());
            }

            if (Input.GetKeyDown(KeyCode.I))
            {
                DeckServiceLocator.GetService<DeckUIService>().SwapStatus<DeckInventoryDisplayer>();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                DeckServiceLocator.GetService<DeckUIService>().ShowWindow<DeckGamePlayUI>();
            }
        }

        private void OnDrawGizmos()
        {
            if (gizmoData == null) return;
            if (!gizmoData.drawGizmos) return;
            // if (!_map) return;
            // _map.DrawGizmos();
        }
    }
}