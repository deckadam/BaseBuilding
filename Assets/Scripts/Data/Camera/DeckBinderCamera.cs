using UnityEngine;
using Zenject;

namespace Deck.Data.Camera
{
	[CreateAssetMenu(menuName = "Deck/Binder/Camera", fileName = "Deck Binder Camera")]
	public class DeckBinderCamera : ScriptableObjectInstaller
	{
		public float CameraMovementSpeed => cameraMovementSpeed;
		[SerializeField] private float cameraMovementSpeed;

		public override void InstallBindings()
		{
			Container.BindInstance(this);
		}
	}
}