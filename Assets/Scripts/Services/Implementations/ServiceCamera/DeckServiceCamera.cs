using Deck.Services;
using UnityEngine;
using Zenject;

namespace Deck.Services.CameraService
{
    public class DeckServiceCamera : DeckServiceBase
    {
        private BoxCollider _generatedCollider;

        private Camera _camera;

        public override void Initialize()
        {
            _camera = Camera.main;
        }

        public Camera GetCamera()
        {
            return _camera;
        }
    }
}