using System.Collections.Generic;
using Deck.Data.Buildable;
using Deck.Services.Building;
using Deck.Utility.Logger;
using Services.Implementations.Escapable;
using DG.Tweening;
using UnityEngine;

namespace Deck.Components.Building.Building
{
    public class DeckBuildingPage : DeckUIElement
    {
        [SerializeField] protected List<DeckBuildable> buildables;

        [SerializeField] protected RectTransform container;

        [SerializeField] protected CanvasGroup canvasGroup;

        protected DeckServiceEscapable EscapableService;
        protected DeckServiceBuilding BuildingService;
        protected DeckBuildingButton Button;
        protected DeckUIBuilding UIBuilding;

        protected override void InternalOnValidate()
        {
            canvasGroup ??= GetComponent<CanvasGroup>();
        }

        public void Initialize(DeckUIBuilding uiBuilding, DeckBuildingButton button)
        {
            UIBuilding = uiBuilding;
            Button = button;
            canvasGroup.interactable = false;
            canvasGroup.alpha = 0f;
            gameObject.SetActive(false);

            EscapableService = Deck.GetService<DeckServiceEscapable>();
            BuildingService = Deck.GetService<DeckServiceBuilding>();

            OnInitialize();
        }

        protected virtual void OnInitialize()
        {
        }

        public void Appear()
        {
            gameObject.SetActive(true);
            canvasGroup.interactable = true;
            canvasGroup.alpha = 1f;
        }

        public void Disappear()
        {
            OnDisappearStart();
            
            if (canvasGroup == null)
            {
                return;
            }
            canvasGroup.interactable = false;
            canvasGroup.alpha = 0f;
            gameObject.SetActive(false);
            OnDisappearEnd();
        }

        protected virtual void OnDisappearStart()
        {
        }


        protected virtual void OnDisappearEnd()
        {
        }

        public void AddBuildable(DeckBuildable buildingData)
        {
            buildables.RemoveAll(item => item == null);
            buildables.Add(buildingData);

            DeckLogger.Inform("Buildable :" + buildingData.Name + "  is added to building list.  " + GetType());
        }
    }
}