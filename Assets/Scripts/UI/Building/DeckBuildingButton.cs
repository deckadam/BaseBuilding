using Base;
using DG.Tweening;
using Services;
using Services.Escapable;
using Services.UI;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Building
{
    public class DeckBuildingButton : DeckUIElement, IDeckEscapable
    {
        [SerializeField] private float movementDuration;
        [SerializeField] private Ease movementEase;
        [SerializeField] private Vector2 appearPosition;
        [SerializeField] private Vector2 disappearPosition;
        [SerializeField] protected Image background;

        public bool HasEscaped { get; private set; } = true;

        private DeckServiceEscapable _escapableService;
        private DeckUIBuilding _uiBuilding;
        private DeckBuildingPage _page;
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
            _uiBuilding = uiBuilding;
            _page = page;
            _escapableService = DeckServiceProvider.GetService<DeckServiceEscapable>();
        }

        public void OnClick()
        {
            _uiBuilding.PageOpenRequested(this);
            _escapableService.RegisterEscapable(this);

            Appear();
            _page.Appear();
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


        public void OnEscapeRequested()
        {
            Disappear();
            _page.Disappear();
        }

        public bool CanBeEscapedWithRightClick => false;
    }
}