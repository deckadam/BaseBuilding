using UnityEngine;

namespace Data.Camera
{
    [CreateAssetMenu(menuName = "Deck/Data/Camera", fileName = "Deck Camera Parameters")]
    public class DeckCameraParameters : ScriptableObject
    {
        [SerializeField] private float cameraMovementSpeedMin;
        [SerializeField] private float cameraMovementSpeedMax;
        [SerializeField] private float minimumHeight;
        [SerializeField] private float maximumHeight;
        [SerializeField] private float zoomSpeed;
        [SerializeField] private float zoomMoveTowardsSpeed;
        [SerializeField] private float rotationSpeed;
        [SerializeField] private float rotationSpeedWithKeyboard;
        [SerializeField] private float rotationResetSpeed;

        public float CameraMovementSpeedMin => cameraMovementSpeedMin;
        public float CameraMovementSpeedMax => cameraMovementSpeedMax;
        public float MinimumHeight => minimumHeight;
        public float MaximumHeight => maximumHeight;
        public float ZoomSpeed => zoomSpeed;
        public float ZoomMoveTowardsSpeed => zoomMoveTowardsSpeed;
        public float RotationSpeed => rotationSpeed;
        public float RotationSpeedWithKeyboard => rotationSpeedWithKeyboard;
        public float RotationResetSpeed => rotationResetSpeed;
    }
}