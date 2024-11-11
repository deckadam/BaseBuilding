using Deck.Data.Map;
using Deck.Services.MapService;
using Deck.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

namespace Deck.Services.Navigation
{
    public class DeckServiceNavigation : DeckServiceBase
    {
        [SerializeField] private NavMeshBuildSettings settings;

        public void GenerateNavigation(DeckBinderMap binderMap)
        {
            var map = DeckServiceScene.GetMap();

            DeckLogger.Navigation("Starting to generate navigation", map.gameObject);

            var navMeshObject = new GameObject() { name = "NavigationSurface" };
            navMeshObject.transform.parent = map.transform;
            navMeshObject.transform.SetSiblingIndex(0);
            var surface = navMeshObject.AddComponent<NavMeshSurface>();

            surface.collectObjects = CollectObjects.Volume;

            surface.layerMask = 1 << 6;
            surface.size = new Vector3(binderMap.GetSize().x, 100f, binderMap.GetSize().y);
            surface.BuildNavMesh();

            DeckLogger.Navigation("Finished generating the navigation", map.gameObject);
        }

        [Button]
        public void SetMinRegion()
        {
            settings.minRegionArea = 0.1f;
        }
    }
}