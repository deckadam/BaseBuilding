using Cinemachine;
using Deck.Data.Camera;
using UnityEngine;
using Zenject;

namespace Deck.CameraController
{
    public class DeckFreeRoamCameraController : MonoBehaviour
    {
        [Inject] private DeckCameraData cameraData;
        [Inject] private CinemachineVirtualCamera _vCam;
        [Inject] private CinemachineConfiner _confiner;
        private new Collider collider;

        private void Update()
        {
            if (_confiner.m_BoundingVolume == null) return;
            collider = _confiner.m_BoundingVolume;

            var movement = Vector3.zero;
            movement += Input.GetAxis("Horizontal") * Vector3.right;
            movement += Input.GetAxis("Vertical") * Vector3.forward;
            var deltaPosition = movement * (Time.deltaTime * cameraData.cameraMovementSpeed);

            if (collider.bounds.Contains(transform.position + deltaPosition))
                transform.position += deltaPosition;
        }
    }
}