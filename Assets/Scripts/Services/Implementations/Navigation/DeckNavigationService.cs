using Deck.Data.Map;
using Deck.Services.Implementations.MapService;
using Deck.Utility.Logger;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace Deck.Services.Implementations.Navigation
{
    public class DeckNavigationService : DeckServiceBase
    {
        [Inject] private DeckMapData mapData;

        public void GenerateNavigation(out NavMeshSurface surface)
        {
            var map = DeckMapService.map;

            DeckLogger.Navigation("Starting to generate navigation", map.gameObject);

            var navMeshObject = new GameObject() {name = "NavigationSurface"};
            navMeshObject.transform.parent = map.transform;
            navMeshObject.transform.SetSiblingIndex(0);
            surface = navMeshObject.AddComponent<NavMeshSurface>();

            surface.collectObjects = CollectObjects.Volume;

            surface.layerMask = 1 << 6;
            surface.size = new Vector3(mapData.size.x, 100f, mapData.size.y);

            surface.BuildNavMesh();
            DeckLogger.Navigation("Finished generating the navigation", map.gameObject);
        }
    }
}