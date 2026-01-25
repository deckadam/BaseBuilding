using Cinemachine;
using Data.Camera;
using EventManager;
using Services;
using Services.Building.Events;
using Services.Camera;
using Services.Map;
using Systems.SystemInput.Events;
using UnityEngine;
using Utility;
using Zenject;

namespace CameraController
{
    public class DeckFreeRoamCameraController : DeckBaseCameraController
    {
        private readonly Vector3 ViewPortMiddle = new(0.5f, 0.5f, 0);
        private DeckCameraParameters _cameraParameters;
        private CinemachineVirtualCamera _vCam;
        private CinemachineConfiner _confiner;
        private Transform _cachedTransform;

        private Vector3 _center;
        private Camera _camera;
        private Plane _groundPlane;
        private bool _canZoom = true;
        private float _targetHeight;
        private float _zoomRatio;
        private bool _rotatingCamera;
        private float _currentRotation;

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
            _camera = DeckServiceProvider.GetService<DeckServiceCamera>().GetCamera();

            _confiner.m_BoundingVolume = DeckServiceProvider.GetService<DeckServiceSession>().GetCurrentSession().GetCameraCollider();
            _targetHeight = DeckServiceProvider.GetService<DeckServiceCamera>().GetCamera().transform.position.y;
            CalculateZoomRatio();

            _groundPlane = new Plane(Vector3.up, Vector3.zero);

            DeckEventManager.Register<DeckEventOnBuildModeStarted>(OnBuildModeStarted);
            DeckEventManager.Register<DeckEventOnBuildModeStopped>(OnBuildModeStopped);
            DeckEventManager.Register<DeckEventOnAxisMovement>(OnAxisMovement);
            DeckEventManager.Register<DeckEventMiddleScroll>(OnMiddleScroll);
            DeckEventManager.Register<DeckEventOnMiddleMouseButtonStatusChange>(OnMiddleMouseButtonStatusChanged);
            DeckEventManager.Register<DeckEventOnCameraRotateWithKeyboard>(OnCameraRotateRequested);
            DeckEventManager.Register<DeckEventOnCameraRotateWithKeyboardStateChanged>(OnCameraRotateStarted);
        }

        private void OnDestroy()
        {
            DeckEventManager.Unregister<DeckEventOnBuildModeStarted>(OnBuildModeStarted);
            DeckEventManager.Unregister<DeckEventOnBuildModeStopped>(OnBuildModeStopped);
            DeckEventManager.Unregister<DeckEventOnAxisMovement>(OnAxisMovement);
            DeckEventManager.Unregister<DeckEventMiddleScroll>(OnMiddleScroll);
            DeckEventManager.Unregister<DeckEventOnMiddleMouseButtonStatusChange>(OnMiddleMouseButtonStatusChanged);
            DeckEventManager.Unregister<DeckEventOnMouseMove>(OnMouseMove);
            DeckEventManager.Unregister<DeckEventOnCameraRotateWithKeyboard>(OnCameraRotateRequested);
            DeckEventManager.Unregister<DeckEventOnCameraRotateWithKeyboardStateChanged>(OnCameraRotateStarted);
        }

        private void OnCameraRotateStarted(DeckEventOnCameraRotateWithKeyboardStateChanged obj)
        {
            if (obj.newState)
            {
                SetCenter();
                _rotatingCamera = true;
            }
            else
            {
                _rotatingCamera = false;
            }
        }

        private void OnCameraRotateRequested(DeckEventOnCameraRotateWithKeyboard obj)
        {
            RotateCamera(obj.direction ? _cameraParameters.RotationSpeedWithKeyboard : -_cameraParameters.RotationSpeedWithKeyboard);
        }

        private void OnMouseMove(DeckEventOnMouseMove obj)
        {
            RotateCamera(obj.delta.x);
        }

        private void RotateCamera(float delta)
        {
            _currentRotation += delta * Time.deltaTime * _cameraParameters.RotationSpeed;
            var direction = (Quaternion.Euler(0, _currentRotation - 180, 0) * Vector3.forward).normalized;
            var position = _cachedTransform.position;
            position.y = 0;
            var distance = Vector3.Distance(_center, position);
            _cachedTransform.position = _center + direction * distance + Vector3.up * _targetHeight;
            _cachedTransform.rotation = Quaternion.Euler(75, _currentRotation, 0);
        }

        private void OnMiddleMouseButtonStatusChanged(DeckEventOnMiddleMouseButtonStatusChange obj)
        {
            _rotatingCamera = obj.status;

            if (_rotatingCamera)
            {
                SetCenter();
                DeckEventManager.Register<DeckEventOnMouseMove>(OnMouseMove);
            }
            else
            {
                DeckEventManager.Unregister<DeckEventOnMouseMove>(OnMouseMove);
            }
        }

        private void SetCenter()
        {
            var ray = _camera.ViewportPointToRay(ViewPortMiddle);
            var isHit = _groundPlane.Raycast(ray, out var distance);
            if (!isHit)
            {
                DeckLogger.Error("Not hitting to ground plane!");
            }

            _currentRotation = _cachedTransform.rotation.eulerAngles.y;
            _center = ray.GetPoint(distance);
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
            if (_rotatingCamera)
            {
                return;
            }

            var movement = Vector3.zero;

            var forwardVector = Quaternion.Euler(0, _currentRotation, 0) * Vector3.forward;
            var rightVector = Quaternion.Euler(0, _currentRotation, 0) * Vector3.right;

            movement += obj.movement.x * rightVector;
            movement += obj.movement.y * forwardVector;

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