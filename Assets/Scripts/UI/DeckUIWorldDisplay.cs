using System;
using Deck.InGame.Agent.Building.Health;
using Deck.Services;
using Deck.Services.CameraService;
using Deck.Utility.Logger;
using UnityEngine;
using Deck.Utility.Poolable;

namespace Deck.InGame.Agent.Building
{
    public class DeckUIWorldDisplay : DeckUIElement
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

        protected override void OnDespawned()
        {
            Deck.GetService<DeckServiceUI>().GetUI<DeckUIWorldLabelDisplayer>().RemoveDisplay(this);
        }

        protected override void OnSpawned()
        {
            Deck.GetService<DeckServiceUI>().GetUI<DeckUIWorldLabelDisplayer>().AddDisplay(this);
        }

        private void LateUpdate()
        {
            try
            {
                rect.position = mainCamera.WorldToScreenPoint(target.transform.position + positionOffset);
            }
            catch (Exception)
            {
                DeckLogger.Inform("Agent system is not working properly. Please check the agent system.");
                uiPool.Return(this);
            }
        }
    }
}