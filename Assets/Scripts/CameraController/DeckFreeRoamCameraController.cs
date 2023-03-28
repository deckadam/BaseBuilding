using Cinemachine;
using Deck.Data.Camera;
using UnityEngine;
using Zenject;

namespace Deck.CameraController
{
    public class DeckFreeRoamCameraController : MonoBehaviour
    {
        private DeckBinderCamera _binderCamera;
        private CinemachineVirtualCamera _vCam;
        private CinemachineConfiner _confiner;

        private void Start()
        {
            _confiner = GetComponent<CinemachineConfiner>();
            _vCam = GetComponent<CinemachineVirtualCamera>();
            _confiner.m_BoundingVolume = GameObject.Find("Camera confiner").GetComponent<Collider>();
        }

        [Inject]
        private void Inject(DeckBinderCamera binderCamera)
        {
            _binderCamera = binderCamera;
        }

        private void Update()
        {
            var movement = Vector3.zero;
            movement += Input.GetAxis("Horizontal") * Vector3.right;
            movement += Input.GetAxis("Vertical") * Vector3.forward;

            var scroll = Input.mouseScrollDelta.y;
            if (scroll > 0 && transform.position.y > _binderCamera.GetMinimumHeight())
            {
                var limit = transform.position.y - _binderCamera.GetMinimumHeight();
                var delta = _binderCamera.GetScrollSpeed() * Time.deltaTime * scroll;
                delta = Mathf.Clamp(delta, 0, limit);
                transform.position += transform.forward * delta;
            }
            else if (scroll < 0 && transform.position.y < _binderCamera.GetMaximumHeight())
            {
                var limit = transform.position.y - _binderCamera.GetMaximumHeight();
                var delta = _binderCamera.GetScrollSpeed() * Time.deltaTime * scroll;
                delta = Mathf.Clamp(delta, limit, 0);
                transform.position += transform.forward * delta;
            }

            var deltaPosition = movement * (Time.deltaTime * _binderCamera.GetCameraMovementSpeed());
            // if (_collider.bounds.Contains(transform.position + deltaPosition))
            transform.position += deltaPosition;
        }
    }
}