using UnityEngine;
using UnityEngine.AI;

namespace Deck.Services.CameraService
{
    public class DeckServiceCamera : DeckServiceBase
    {
        private BoxCollider _generatedCollider;

        private Camera _camera;
        private Plane _groundPlane;

        public Vector3 GetCursorWorldPosition()
        {
            var screenPosition = Input.mousePosition;
            var ray = _camera.ScreenPointToRay(screenPosition);

            if (_groundPlane.Raycast(ray, out var distance))
            {
                return ray.GetPoint(distance);
            }

            return Vector3.zero;
        }

        public override void Initialize()
        {
            _camera = Camera.main;
            _groundPlane = new Plane(Vector3.up, Vector3.zero);
        }

        public Camera GetCamera()
        {
            return _camera;
        }
    }
}