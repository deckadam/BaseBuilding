using System.Collections.Generic;
using Deck.Components;
using Deck.Data.Buildable;
using Deck.Services.Building;
using Deck.Services.Implementations.Escapable;
using Deck.UI.Building.BuildMode;
using Deck.Utility;
using UI.Building.BuildMode;
using UnityEngine;

namespace Deck.UI.Building
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

            escapableService = Deck.GetService<DeckServiceEscapable>();
            BuildingService = Deck.GetService<DeckServiceBuilding>();

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

            DeckLogger.Inform("Buildable :" + buildingData.Name + "  is added to building list.  " + GetType());
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
            BuildingService.Clear();
        }

        protected virtual void InternalInitialize()
        {
            
        }
    }
}