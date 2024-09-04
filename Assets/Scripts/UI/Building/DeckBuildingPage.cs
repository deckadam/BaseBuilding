using Deck.Services.Building;
using Deck.Utility.Poolable;
using DG.Tweening;
using Services.Implementations.Escapable;
using UnityEngine;

namespace Deck.InGame.Agent.Building.Building
{
    public class DeckBuildingPage : DeckUIElement
    {
        [SerializeField] protected RectTransform container;
        
        [SerializeField] protected CanvasGroup canvasGroup;
        [SerializeField] protected float appearDuration;
        [SerializeField] protected Ease ease;

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
            canvasGroup.DOKill();
            canvasGroup.DOFade(1f, appearDuration).SetEase(ease);
        }

        public void Disappear()
        {
            OnDisappearStart();
            canvasGroup.interactable = false;
            canvasGroup.DOKill();
            canvasGroup.DOFade(0f, appearDuration).SetEase(ease);
            gameObject.SetActive(false);
            OnDisappearEnd();
        }

        protected virtual void OnDisappearStart()
        {
            
        }
        
        
        protected virtual void OnDisappearEnd()
        {
            
        }
    }
}