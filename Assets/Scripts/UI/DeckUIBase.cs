using Cysharp.Threading.Tasks;
using Deck.Data.UI;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace Deck.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class DeckUIBase : MonoBehaviour
    {
        protected DiContainer container;
        protected DeckBinderUI binderUI;

        [SerializeField] protected CanvasGroup canvasGroup;
        [SerializeField] protected bool isAppearedOnStartUp;
        [SerializeField] protected RectTransform rectTransform;

        // [ShowInInspector, ReadOnly] internal bool _isAppeared;
        [SerializeField] internal bool _isAppeared;
        internal bool isAppearing;
        internal bool isDisappearing;

        [Inject]
        private void Inject(DiContainer container, DeckBinderUI binderUI)
        {
            this.container = container;
            this.binderUI = binderUI;
        }

        private void OnValidate()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            rectTransform = GetComponent<RectTransform>();
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