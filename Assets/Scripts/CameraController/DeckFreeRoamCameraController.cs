using Cinemachine;
using Deck.Data.Camera;
using Deck.EventManager;
using Deck.Services.Building.Events;
using UnityEngine;
using Zenject;

namespace Deck.CameraController
{
    public class DeckFreeRoamCameraController : MonoBehaviour
    {
        private DeckBinderCamera _binderCamera;
        private CinemachineVirtualCamera _vCam;
        private CinemachineConfiner _confiner;
        private bool _canZoom = true;

        private void Start()
        {
            _confiner = GetComponent<CinemachineConfiner>();
            _vCam = GetComponent<CinemachineVirtualCamera>();
            _confiner.m_BoundingVolume = GameObject.Find("Camera confiner").GetComponent<Collider>();
            _confiner.m_BoundingVolume.isTrigger = true;
            DeckEventManager.Register<DeckEventOnBuildModeStarted>(OnBuildModeStarted);
            DeckEventManager.Register<DeckEventOnBuildModeStopped>(OnBuildModeStopped);
        }

        private void OnDestroy()
        {
            DeckEventManager.Unregister<DeckEventOnBuildModeStarted>(OnBuildModeStarted);
            DeckEventManager.Unregister<DeckEventOnBuildModeStopped>(OnBuildModeStopped);
        }

        private void OnBuildModeStarted(DeckEventOnBuildModeStarted obj)
        {
            _canZoom = false;
        }

        private void OnBuildModeStopped(DeckEventOnBuildModeStopped obj)
        {
            _canZoom = true;
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

            if (_canZoom)
            {
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
            }

            var deltaPosition = movement * (Time.deltaTime * _binderCamera.GetCameraMovementSpeed());
            transform.position += deltaPosition;
        }
    }
}