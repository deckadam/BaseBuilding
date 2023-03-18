using Cinemachine;
using Deck.Map;
using UnityEngine;
using Zenject;

namespace Deck.Services.Implementations.CameraService
{
    public class DeckCameraService : DeckServiceBase
    {
        private BoxCollider _generatedCollider;

        private CinemachineConfiner _confiner;
        private CinemachineBrain _brain;
        private Camera _camera;

        [Inject]
        private void Inject(CinemachineConfiner confiner, CinemachineBrain brain, Camera camera)
        {
            _confiner = confiner;
            _brain = brain;
            _camera = camera;
        }

        public void GenerateCameraBounds(DeckCoreGrid deckCoreGrid)
        {
            if (_generatedCollider != null)
            {
                Destroy(_generatedCollider.gameObject);
            }

            var temp = new GameObject();
            temp.name = "Camera confiner";
            _generatedCollider = temp.AddComponent<BoxCollider>();
            var size = new Vector3(deckCoreGrid.size.x, 100f, deckCoreGrid.size.y);
            _generatedCollider.size = size;
            _generatedCollider.center = Vector3.zero;

            _confiner.m_BoundingVolume = _generatedCollider;
        }

        public Camera GetCamera()
        {
            return _camera;
        }
    }
}