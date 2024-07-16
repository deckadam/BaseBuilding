using Cysharp.Threading.Tasks;
using Deck.Utility.Poolable;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Deck.UI.Building
{
    public class DeckBuildingButton : DeckUIElement
    {
        [SerializeField] private float movementDuration;
        [SerializeField] private Ease movementEase;

        [SerializeField] private Vector2 appearPosition;
        [SerializeField] private Vector2 disappearPosition;

        [SerializeField] protected Image background;

        protected DeckBuildingUI buildingUI;

        protected DeckBuildingPage page;

        private bool _isAppeared;

        public void Initialize(DeckBuildingUI buildingUI, DeckBuildingPage page)
        {
            this.buildingUI = buildingUI;
            this.page = page;
            OnInitialize();
        }

        protected virtual void OnInitialize()
        {
        }

        public void OnClick()
        {
            buildingUI.OnBuildingSetButtonClicked(this);
            Appear().Forget();
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
    }
}