using Deck.Data.Gizmo;
using Deck.Data.Item;
using Deck.EventManager;
using Deck.InputHandling.Events;
using Deck.Inventory.UI;
using Deck.Player;
using Deck.Services;
using Deck.Services.Implementations.CameraService;
using Deck.Services.Implementations.CellSelectionService;
using Deck.Services.Implementations.GridService;
using Deck.Services.Implementations.Level;
using Deck.Services.Implementations.MapService;
using Deck.Services.Implementations.Navigation;
using Deck.Utility;
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
            Debug.LogError("Agent selected");
            _lastSelectedAgent = obj.agent;
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

            if (Input.GetKeyDown(KeyCode.A))
            {
                _lastSelectedAgent.GetInventory().AddItem(itemData.items.GetRandom());
            }

            if (Input.GetKeyDown(KeyCode.I))
            {
                inventoryDisplayer.SetItems(_lastSelectedAgent);
                inventoryDisplayer.gameObject.SetActive(true);
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