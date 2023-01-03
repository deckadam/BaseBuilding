using UnityEngine;
using Zenject;

namespace Deck.Data.Camera
{
    [CreateAssetMenu(menuName = "Deck/Installer/Camera", fileName = "Deck Camera Data")]
    public class DeckCameraData : ScriptableObjectInstaller
    {
        public float cameraMovementSpeed;

        public override void InstallBindings()
        {
            Container.BindInstance(this);
        }
    }
}