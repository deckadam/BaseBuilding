using Deck.Components;
using Deck.Services;
using DG.Tweening;
using Services.Implementations.Escapable;
using UnityEngine;
using UnityEngine.UI;

namespace Deck.Components.Building.Building
{
    public class DeckBuildingButton : DeckUIElement, IDeckEscapable
    {
        [SerializeField] private float movementDuration;
        [SerializeField] private Ease movementEase;

        [SerializeField] private Vector2 appearPosition;
        [SerializeField] private Vector2 disappearPosition;

        [SerializeField] protected Image background;

        public bool HasEscaped { get; private set; } = true;

        protected DeckUIBuilding uiBuilding;
        protected DeckBuildingPage page;

        private DeckServiceEscapable _escapableService;
        private bool _isAppeared;

        public DeckBuildingButton(bool hasEscaped)
        {
            HasEscaped = hasEscaped;
        }

        protected DeckBuildingButton()
        {
        }

        public void Initialize(DeckUIBuilding uiBuilding, DeckBuildingPage page)
        {
            this.uiBuilding = uiBuilding;
            this.page = page;
            _escapableService = Deck.GetService<DeckServiceEscapable>();
            OnInitialize();
        }

        protected virtual void OnInitialize()
        {
        }

        public void OnClick()
        {
            uiBuilding.PageOpenRequested(this);
            _escapableService.RegisterEscapable(this);

            Appear();
            page.Appear();
        }

        private void Appear()
        {
            if (_isAppeared)
            {
                return;
            }

            _isAppeared = true;
            background.rectTransform.DOAnchorPos(appearPosition, movementDuration).SetEase(movementEase);
            HasEscaped = false;
        }

        private void Disappear()
        {
            if (!_isAppeared)
            {
                return;
            }

            _isAppeared = false;

            background.rectTransform.DOAnchorPos(disappearPosition, movementDuration).SetEase(movementEase);
            HasEscaped = true;
        }


        public void OnCloseRequested()
        {
            Disappear();
            page.Disappear();
        }
    }
}