using Deck.EventManager;
using Deck.InputHandling.Events;
using UnityEngine;
using UnityEngine.AI;
using Deck.Utility;

namespace Deck.Map
{
    public class DeckMap : MonoBehaviour
    {
        private DeckCoreGrid _deckCoreGrid;
        private NavMeshSurface _surface;
        private GameObject _ground;
        private MeshRenderer _renderer;

        private void OnEnable()
        {
            DeckEventManager.Register<DeckOnGroundPositionChangeEvent>(UpdateGroundMaterial);
        }

        private void OnDisable()
        {
            DeckEventManager.Unregister<DeckOnGroundPositionChangeEvent>(UpdateGroundMaterial);
        }

        private void UpdateGroundMaterial(DeckOnGroundPositionChangeEvent obj)
        {
            _renderer.material.SetVector(DeckShaderConstants.MousePos, new Vector4(-obj.position.x + 0.5f, -obj.position.y + 0.5f, 0, 0));
        }


        public void Initialize(DeckCoreGrid deckCoreGrid, NavMeshSurface surface, GameObject ground)
        {
            _deckCoreGrid = deckCoreGrid;
            _surface = surface;
            _renderer = ground.GetComponent<MeshRenderer>();
            _renderer.material.SetFloat(DeckShaderConstants.Range, 1f / deckCoreGrid.size.x / 2f);
        }

        public void DrawGizmos()
        {
            for (int i = 0; i < _deckCoreGrid.cells.GetLength(0); i++)
            {
                for (int j = 0; j < _deckCoreGrid.cells.GetLength(1); j++)
                {
                    Gizmos.DrawSphere(new Vector3(i, 0, j), 0.2f);
                }
            }
        }

        public DeckCoreGrid GetGrid() => _deckCoreGrid;
        public NavMeshSurface GetSurface() => _surface;
    }
}