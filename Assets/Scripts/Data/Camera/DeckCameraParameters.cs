using UnityEngine;

namespace Data.Camera
{
    [CreateAssetMenu(menuName = "Deck/Data/Camera", fileName = "Deck Camera Parameters")]
    public class DeckCameraParameters : ScriptableObject
    {
        [SerializeField] private float cameraMovementSpeed;
        [SerializeField] private float minimumHeight;
        [SerializeField] private float maximumHeight;
        [SerializeField] private float zoomSpeed;
        [SerializeField] private float zoomMoveTowardsSpeed;

        public float CameraMovementSpeed => cameraMovementSpeed;
        public float MinimumHeight => minimumHeight;
        public float MaximumHeight => maximumHeight;
        public float ZoomSpeed => zoomSpeed;
        public float ZoomMoveTowardsSpeed => zoomMoveTowardsSpeed;
    }
}