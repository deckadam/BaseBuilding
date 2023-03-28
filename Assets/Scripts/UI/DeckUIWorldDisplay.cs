using Deck.Events.CameraService;
using UnityEngine;

namespace Deck.UI
{
    public class DeckUIWorldDisplay : MonoBehaviour
    {
        protected Camera mainCamera;
        protected Transform target;
        protected RectTransform rect;
        protected Vector3 positionOffset;

        private void Awake()
        {
            rect = GetComponent<RectTransform>();
            Initialize();
        }

        protected virtual void Initialize()
        {
        }

        public void SetTarget(Transform target)
        {
            mainCamera = Deck.GetService<DeckServiceCamera>().GetCamera();
            this.target = target;
        }

        public void SetPositionOffset(Vector3 offset)
        {
            positionOffset = offset;
        }


        private void LateUpdate()
        {
            rect.position = mainCamera.WorldToScreenPoint(target.transform.position + positionOffset);
        }
    }
}