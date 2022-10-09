using Deck.Map;
using Deck.Services;
using Deck.Services.Implementations.MapService;
using UnityEngine;

namespace DefaultNamespace
{
    public class GameTest : MonoBehaviour
    {
        public Vector2Int size;
        public bool drawGizmos;
        private GamePlayMap _map;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                if (_map) Destroy(_map.gameObject);
                _map = ServiceLocator.GetService<MapService>().CreateMap("test map", size.x, size.y);
            }
        }

        private void OnDrawGizmos()
        {
            if (!drawGizmos) return;
            if (!_map) return;
            _map.DrawGizmos();
        }
    }
}