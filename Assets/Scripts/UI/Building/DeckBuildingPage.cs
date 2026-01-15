using System.Collections.Generic;
using Data.Buildable;
using Deck.Base;
using Services.Building;
using Services.Escapable;
using UI.Building.BuildMode;
using UnityEngine;
using Utility;

namespace UI.Building
{
    public class DeckBuildingPage : DeckUIElement
    {
        [SerializeField] protected List<DeckBuildable> buildables;
        [SerializeField] protected RectTransform container;
        [SerializeField] protected CanvasGroup canvasGroup;

        protected DeckServiceBuilding BuildingService;

        private DeckEscapableBuildMode escapableBuildMode;
        private DeckServiceEscapable escapableService;

        private void OnDestroy()
        {
            escapableBuildMode?.OnEscapeRequested();
        }

        protected sealed override void InternalOnValidate()
        {
            canvasGroup ??= GetComponent<CanvasGroup>();
        }

        public void Initialize()
        {
            canvasGroup.interactable = false;
            canvasGroup.alpha = 0f;
            gameObject.SetActive(false);

            escapableService = Services.DeckServiceProvider.GetService<DeckServiceEscapable>();
            BuildingService = Services.DeckServiceProvider.GetService<DeckServiceBuilding>();

            foreach (var buildable in buildables)
            {
                var buildableButton = InstanceProvider.RentUIElement<DeckBuildableButton>();
                buildableButton.Initialize(this, buildable);
                buildableButton.rectTransform.SetParent(container, false);
            }

            InternalInitialize();
        }

        public void Appear()
        {
            gameObject.SetActive(true);
            canvasGroup.interactable = true;
            canvasGroup.alpha = 1f;
        }

        public void Disappear()
        {
            if (canvasGroup == null)
            {
                return;
            }

            canvasGroup.interactable = false;
            canvasGroup.alpha = 0f;
            gameObject.SetActive(false);
        }

        public void AddBuildable(DeckBuildable buildingData)
        {
            buildables.RemoveAll(item => item == null);
            buildables.Add(buildingData);

            DeckLogger.Inform("Buildable :" + buildingData.VisibleName + "  is added to building list.  " + GetType());
        }

        public void OnBuildableSelected(DeckBuildable buildable)
        {
            if (escapableBuildMode != null)
            {
                escapableService.RemoveEscapable(escapableBuildMode);
            }

            escapableBuildMode = DeckEscapableBuildModeProvider.GetEscapableBuildMode(buildable.BuildMode);
            escapableBuildMode.Initialize(OnEscapeRequested, buildable, this);
        }

        public virtual Quaternion GetBuildableRotation(DeckBuildable buildable)
        {
            return Quaternion.identity;
        }

        private void OnEscapeRequested()
        {
            escapableBuildMode = null;
            BuildingService.ClearAll();
        }

        protected virtual void InternalInitialize()
        {
        }
    }
}