using UnityEngine;
using Utility;

namespace Services.Camera
{
    public class DeckServiceCamera : DeckServiceBase
    {
        private BoxCollider _generatedCollider;

        private UnityEngine.Camera _camera;
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


        public Vector2Int GetCursorCellIndex()
        {
            var screenPosition = Input.mousePosition;
            var ray = _camera.ScreenPointToRay(screenPosition);

            if (_groundPlane.Raycast(ray, out var distance))
            {
                return ray.GetPoint(distance).ToVector2Int();
            }

            return Vector2Int.zero;
        }

        public override void Initialize()
        {
            _camera = UnityEngine.Camera.main;
            _groundPlane = new Plane(Vector3.up, Vector3.zero);
        }

        public Ray GetRayFromCamera()
        {
            return _camera.ScreenPointToRay(Input.mousePosition);
        }

        public UnityEngine.Camera GetCamera()
        {
            return _camera;
        }
    }
}