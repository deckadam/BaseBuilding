using Cysharp.Threading.Tasks;
using Deck.Data.UI;
using Deck.Components;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Deck.Components.Building
{
    [RequireComponent(typeof(CanvasGroup))]
    public class DeckUIBase : DeckUIElement
    {
        protected DeckBinderUI binderUI;

        [SerializeField] protected CanvasGroup canvasGroup;
        [SerializeField] protected bool isAppearedOnStartUp;

        // [ShowInInspector, ReadOnly] internal bool _isAppeared;
        [SerializeField] internal bool _isAppeared;
        internal bool isAppearing;
        internal bool isDisappearing;

        [Inject]
        private void Inject(DeckBinderUI binderUI)
        {
            this.binderUI = binderUI;
        }

        [Button]
        private void OnValidate()
        {
            canvasGroup ??= GetComponent<CanvasGroup>();
            rectTransform ??= GetComponent<RectTransform>();
            AfterValidate();
        }

        protected virtual void AfterValidate()
        {
        }

        private void Awake()
        {
            _isAppeared = isAppearedOnStartUp;
            if (_isAppeared)
            {
                canvasGroup.alpha = 1f;
                _isAppeared = true;
                canvasGroup.blocksRaycasts = true;
                canvasGroup.interactable = true;
            }
            else
            {
                canvasGroup.alpha = 0f;
                _isAppeared = false;
                canvasGroup.blocksRaycasts = false;
                canvasGroup.interactable = false;
            }
        }

        public async void SwapAppearanceStatus()
        {
            if (_isAppeared)
            {
                await Disappear();
            }
            else
            {
                await Appear();
            }
        }

        public async UniTask Appear()
        {
            isAppearing = true;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;

            OnPreAppear();
            await canvasGroup.DOFade(1f, binderUI.GetCanvasAppearDuration()).AsyncWaitForCompletion();

            _isAppeared = true;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
            isAppearing = false;

            OnPostAppear();
        }

        public async UniTask Disappear()
        {
            if (!CanDisappear())
            {
                return;
            }

            isDisappearing = true;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            OnPreDisappear();
            await canvasGroup.DOFade(0f, binderUI.GetCanvasDisappearDuration()).AsyncWaitForCompletion();
            _isAppeared = false;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            isDisappearing = false;
            OnPostDisappear();
        }

        public RectTransform GetRectTransform()
        {
            return rectTransform;
        }

        public virtual void Initialize()
        {
        }

        public virtual void DeInitialize()
        {
        }

        protected virtual void OnPreAppear()
        {
        }

        protected virtual void OnPostAppear()
        {
        }

        protected virtual void OnPreDisappear()
        {
        }

        protected virtual void OnPostDisappear()
        {
        }

        protected virtual bool CanDisappear()
        {
            return true;
        }

        public virtual void OnCloseRequested()
        {
            Disappear().Forget();
        }
    }
}