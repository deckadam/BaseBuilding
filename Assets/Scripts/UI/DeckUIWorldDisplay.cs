using Deck.Services;
using Deck.Services.CameraService;
using Deck.UI.Health;
using UnityEngine;
using Deck.Utility.Poolable;

namespace Deck.UI
{
    public class DeckUIWorldDisplay : DeckPoolable
    {
        [SerializeField] protected Transform target;
        protected Camera mainCamera;
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

        protected override void Despawned()
        {
            Deck.GetService<DeckServiceUI>().GetUI<DeckUIWorldLabelDisplayer>().RemoveDisplay(this);
        }

        protected override void Spawned()
        {
            Deck.GetService<DeckServiceUI>().GetUI<DeckUIWorldLabelDisplayer>().AddDisplay(this);
        }

        private void LateUpdate()
        {
            if (target == null || !target.gameObject || mainCamera == null || !mainCamera.gameObject)
            {
                Despawn();
            }

            rect.position = mainCamera.WorldToScreenPoint(target.transform.position + positionOffset);
        }
    }
}