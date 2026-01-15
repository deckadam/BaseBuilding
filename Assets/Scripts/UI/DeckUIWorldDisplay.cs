using System;
using Deck.Base;
using Deck.Services.Cam;
using Deck.Services.UI;
using Deck.UI.Health;
using Deck.Utility;
using UnityEngine;
using Utility;

namespace UI
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
            mainCamera = Services.DeckServiceProvider.GetService<DeckServiceCamera>().GetCamera();
            this.target = target;
        }

        public void SetPositionOffset(Vector3 offset)
        {
            positionOffset = offset;
        }

        protected override void InternalOnDeSpawned()
        {
            Services.DeckServiceProvider.GetService<DeckServiceUI>().GetUI<DeckUIWorldLabelDisplayer>().RemoveDisplay(this);
        }

        protected override void InternalOnSpawned()
        {
            Services.DeckServiceProvider.GetService<DeckServiceUI>().GetUI<DeckUIWorldLabelDisplayer>().AddDisplay(this);
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
                InstanceProvider.ReturnUIElement(this);
            }
        }
    }
}