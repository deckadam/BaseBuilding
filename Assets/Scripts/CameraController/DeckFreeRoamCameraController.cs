using Cinemachine;
using Deck.Data.Camera;
using UnityEngine;
using Zenject;

namespace Deck.CameraController
{
	public class DeckFreeRoamCameraController : MonoBehaviour
	{
		private Collider _collider;

		private DeckBinderCamera _binderCamera;
		private CinemachineVirtualCamera _vCam;
		private CinemachineConfiner _confiner;

		[Inject]
		private void Inject(DeckBinderCamera binderCamera, CinemachineVirtualCamera vCam, CinemachineConfiner confiner)
		{
			_binderCamera = binderCamera;
			_vCam = vCam;
			_confiner = confiner;
		}

		private void Update()
		{
			if (_confiner.m_BoundingVolume == null) return;
			_collider = _confiner.m_BoundingVolume;

			var movement = Vector3.zero;
			movement += Input.GetAxis("Horizontal") * Vector3.right;
			movement += Input.GetAxis("Vertical") * Vector3.forward;
			var deltaPosition = movement * (Time.deltaTime * _binderCamera.CameraMovementSpeed);
			if (_collider.bounds.Contains(transform.position + deltaPosition))
				transform.position += deltaPosition;
		}
	}
}