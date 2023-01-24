using Cinemachine;
using Deck.Map;
using UnityEngine;
using Zenject;

namespace Deck.Services.Implementations.CameraService
{
    public class DeckCameraService : DeckServiceBase
    {
        [Inject] private CinemachineConfiner confiner;
        private BoxCollider _generatedCollider;

        public void GenerateCameraBounds(DeckCoreGrid deckCoreGrid)
        {
            if (_generatedCollider != null)
            {
                Destroy(_generatedCollider.gameObject);
            }

            var temp = new GameObject();
            _generatedCollider = temp.AddComponent<BoxCollider>();
            var size = new Vector3(deckCoreGrid.size.x, 100f, deckCoreGrid.size.y);
            _generatedCollider.size = size;
            _generatedCollider.center = Vector3.zero;

            confiner.m_BoundingVolume = _generatedCollider;
        }
    }
}