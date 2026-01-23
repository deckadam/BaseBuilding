using UnityEngine;
using UnityEngine.Serialization;

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

        public float CameraMovementSpeedMin => cameraMovementSpeedMin;
        public float CameraMovementSpeedMax => cameraMovementSpeedMax;
        public float MinimumHeight => minimumHeight;
        public float MaximumHeight => maximumHeight;
        public float ZoomSpeed => zoomSpeed;
        public float ZoomMoveTowardsSpeed => zoomMoveTowardsSpeed;
    }
}