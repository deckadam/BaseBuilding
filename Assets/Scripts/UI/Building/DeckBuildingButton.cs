using Cysharp.Threading.Tasks;
using Deck.Services;
using Deck.Utility.Poolable;
using DG.Tweening;
using Services.Implementations.Escapable;
using UnityEngine;
using UnityEngine.UI;

namespace Deck.UI.Building
{
    public class DeckBuildingButton : DeckUIElement, IDeckEscapable
    {
        [SerializeField] private float movementDuration;
        [SerializeField] private Ease movementEase;

        [SerializeField] private Vector2 appearPosition;
        [SerializeField] private Vector2 disappearPosition;

        [SerializeField] protected Image background;

        protected DeckBuildingUI buildingUI;
        protected DeckBuildingPage page;

        private DeckServiceEscapable _escapableService;
        private bool _isAppeared;

        public void Initialize(DeckBuildingUI buildingUI, DeckBuildingPage page)
        {
            this.buildingUI = buildingUI;
            this.page = page;
            _escapableService = Deck.GetService<DeckServiceEscapable>();
            OnInitialize();
        }

        protected virtual void OnInitialize()
        {
        }

        public void OnClick()
        {
            _escapableService.RegisterEscapable(this);
            
            Appear().Forget();
            page.Appear().Forget();
        }

        protected virtual async UniTask Appear()
        {
            if (_isAppeared)
            {
                return;
            }

            _isAppeared = true;
            await background.rectTransform.DOAnchorPos(appearPosition, movementDuration).SetEase(movementEase).AsyncWaitForCompletion();
        }

        public virtual async UniTask Disappear()
        {
            if (!_isAppeared)
            {
                return;
            }

            _isAppeared = false;

            await background.rectTransform.DOAnchorPos(disappearPosition, movementDuration).SetEase(movementEase).AsyncWaitForCompletion();
        }

        public void OnCloseRequested()
        {
            Disappear().Forget();
            page.Disappear().Forget();
        }
    }
}