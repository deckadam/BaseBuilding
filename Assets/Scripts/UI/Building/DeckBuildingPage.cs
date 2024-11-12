using System.Collections.Generic;
using Deck.Components;
using Deck.Data.Buildable;
using Deck.Services.Building;
using Deck.UI.Building.BuildingSets.BuildMode;
using Deck.Utility;
using Services.Implementations.Escapable;
using UnityEngine;

namespace Deck.UI.Building
{
    public class DeckBuildingPage : DeckUIElement
    {
        [SerializeField] protected List<DeckBuildable> buildables;

        [SerializeField] protected RectTransform container;

        [SerializeField] protected CanvasGroup canvasGroup;

        protected DeckEscapableBuildMode escapableBuildMode;
        protected DeckServiceEscapable escapableService;
        protected DeckServiceBuilding buildingService;
        protected DeckBuildingButton button;
        protected DeckUIBuilding uIBuilding;
        protected bool isBuildModeActive;

        protected override void InternalOnValidate()
        {
            canvasGroup ??= GetComponent<CanvasGroup>();
        }

        public void Initialize(DeckUIBuilding uiBuilding, DeckBuildingButton button)
        {
            uIBuilding = uiBuilding;
            this.button = button;
            canvasGroup.interactable = false;
            canvasGroup.alpha = 0f;
            gameObject.SetActive(false);

            escapableService = Deck.GetService<DeckServiceEscapable>();
            buildingService = Deck.GetService<DeckServiceBuilding>();

            InternalOnInitialize();
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

        protected void OnEscapeRequested()
        {
            isBuildModeActive = false;
            escapableBuildMode = null;
            buildingService.Clear();
            InternalOnEscapeRequested();
        }

        protected virtual void InternalOnInitialize()
        {
        }

        protected virtual void InternalOnEscapeRequested()
        {
        }
    }
}