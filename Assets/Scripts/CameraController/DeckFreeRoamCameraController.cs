using Cinemachine;
using Data.Camera;
using EventManager;
using Services;
using Services.Building.Events;
using Services.Camera;
using Services.Map;
using Systems.SystemInput.Events;
using UnityEngine;
using Zenject;

namespace CameraController
{
    public class DeckFreeRoamCameraController : DeckBaseCameraController
    {
        private DeckCameraParameters _cameraParameters;
        private CinemachineVirtualCamera _vCam;
        private CinemachineConfiner _confiner;
        private Transform _cachedTransform;

        private bool _canZoom = true;
        private float _targetHeight;
        private float _zoomRatio;

        [Inject]
        private void Inject(DeckCameraParameters cameraParameters)
        {
            _cameraParameters = cameraParameters;
        }

        public override void Initialize()
        {
            _cachedTransform = transform;
            _confiner = GetComponent<CinemachineConfiner>();
            _vCam = GetComponent<CinemachineVirtualCamera>();

            _confiner.m_BoundingVolume = DeckServiceProvider.GetService<DeckServiceSession>().GetCurrentSession().GetCameraCollider();
            _targetHeight = DeckServiceProvider.GetService<DeckServiceCamera>().GetCamera().transform.position.y;
            CalculateZoomRatio();

            DeckEventManager.Register<DeckEventOnBuildModeStarted>(OnBuildModeStarted);
            DeckEventManager.Register<DeckEventOnBuildModeStopped>(OnBuildModeStopped);
            DeckEventManager.Register<DeckEventOnAxisMovement>(OnAxisMovement);
            DeckEventManager.Register<DeckEventMiddleScroll>(OnMiddleScroll);
        }


        private void OnDestroy()
        {
            DeckEventManager.Unregister<DeckEventOnBuildModeStarted>(OnBuildModeStarted);
            DeckEventManager.Unregister<DeckEventOnBuildModeStopped>(OnBuildModeStopped);
            DeckEventManager.Unregister<DeckEventOnAxisMovement>(OnAxisMovement);
            DeckEventManager.Unregister<DeckEventMiddleScroll>(OnMiddleScroll);
        }

        private void OnBuildModeStarted(DeckEventOnBuildModeStarted obj)
        {
            _canZoom = false;
        }

        private void OnBuildModeStopped(DeckEventOnBuildModeStopped obj)
        {
            _canZoom = true;
        }

        private void OnAxisMovement(DeckEventOnAxisMovement obj)
        {
            var movement = Vector3.zero;
            movement += obj.movement.x * Vector3.right;
            movement += obj.movement.y * Vector3.forward;
            var speedMultiplier = Mathf.Lerp(_cameraParameters.CameraMovementSpeedMin, _cameraParameters.CameraMovementSpeedMax, _zoomRatio);
            var deltaPosition = movement * (Time.deltaTime * speedMultiplier);
            _cachedTransform.position += deltaPosition;
        }

        private void OnMiddleScroll(DeckEventMiddleScroll obj)
        {
            if (!_canZoom)
            {
                return;
            }

            _targetHeight -= obj.scrollValue * _cameraParameters.ZoomSpeed;
            _targetHeight = Mathf.Clamp(_targetHeight, _cameraParameters.MinimumHeight, _cameraParameters.MaximumHeight);

            CalculateZoomRatio();
        }

        private void Update()
        {
            var currentPosition = _cachedTransform.position;
            var currentHeight = currentPosition.y;
            var heightDifference = currentHeight - _targetHeight;
            _cachedTransform.position = Vector3.MoveTowards(currentPosition, currentPosition + _cachedTransform.forward * heightDifference, _cameraParameters.ZoomMoveTowardsSpeed);
        }

        private void CalculateZoomRatio()
        {
            var range = _cameraParameters.MaximumHeight - _cameraParameters.MinimumHeight;
            var offset = _targetHeight - _cameraParameters.MinimumHeight;
            _zoomRatio = offset / range;
        }
    }
}