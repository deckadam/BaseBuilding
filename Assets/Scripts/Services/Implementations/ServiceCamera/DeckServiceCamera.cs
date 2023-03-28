using Cinemachine;
using Deck;
using Deck.Data.Map;
using Deck.Services;
using UnityEngine;

namespace Deck.Events.CameraService
{
    public class DeckServiceCamera : DeckServiceBase
    {
        private BoxCollider _generatedCollider;

        private Camera _camera;

        public override void Initialize()
        {
            _camera = Camera.main;
        }

        public void GenerateCameraBounds(DeckBinderMap binderMap)
        {
            if (_generatedCollider != null)
            {
                Destroy(_generatedCollider.gameObject);
            }

            var temp = new GameObject();
            temp.name = "Camera confiner";
            temp.layer = 31;
            temp.transform.parent = FindObjectOfType<DeckMap>().transform;
            _generatedCollider = temp.AddComponent<BoxCollider>();
            var size = new Vector3(binderMap.GetSize().x, 100f, binderMap.GetSize().y);
            _generatedCollider.size = size;
            _generatedCollider.center = Vector3.zero;

            FindObjectOfType<CinemachineConfiner>().m_BoundingVolume = _generatedCollider;
        }

        public Camera GetCamera()
        {
            return _camera;
        }
    }
}