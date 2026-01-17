using UnityEngine;
using Zenject;

namespace Data.Camera
{
    [CreateAssetMenu(menuName = "Deck/Binder/Camera", fileName = "Deck Binder Camera")]
    public class DeckBinderCamera : ScriptableObjectInstaller
    {
        [SerializeField] private float getCameraMovementSpeed;
        [SerializeField] private float scrollSpeed;
        [SerializeField] private float minimumHeight;
        [SerializeField] private float maximumHeight;

        public override void InstallBindings()
        {
            Container.BindInstance(this);
        }

        public float GetCameraMovementSpeed() => getCameraMovementSpeed;
        public float GetMinimumHeight() => minimumHeight;
        public float GetMaximumHeight() => maximumHeight;
        public float GetScrollSpeed() => scrollSpeed;
    }
}