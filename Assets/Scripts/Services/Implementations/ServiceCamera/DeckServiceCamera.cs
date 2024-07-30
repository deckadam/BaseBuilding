using UnityEngine;
using UnityEngine.AI;

namespace Deck.Services.CameraService
{
    public class DeckServiceCamera : DeckServiceBase
    {
        private BoxCollider _generatedCollider;

        private Camera _camera;

        public Vector3 GetCursorWorldPosition()
        {
            var screenPosition = Input.mousePosition;
            var ray = _camera.ScreenPointToRay(screenPosition);
            var positionOnGroundPlane = ray.origin - ray.direction / ray.direction.y * ray.origin.y; //collide with plane at y=0

            if (!NavMesh.SamplePosition(positionOnGroundPlane, out var navMeshHit, 1, 1))
            {
                return Vector3.zero;
            }

            return navMeshHit.position;
        }

        public override void Initialize()
        {
            _camera = Camera.main;
        }

        public Camera GetCamera()
        {
            return _camera;
        }
    }
}