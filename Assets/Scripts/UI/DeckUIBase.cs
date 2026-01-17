using Base;
using Cysharp.Threading.Tasks;
using Data.UI;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class DeckUIBase : DeckUIElement
    {
        protected DeckBinderUI binderUI;

        [SerializeField] protected CanvasGroup canvasGroup;
        [SerializeField] protected bool isAppearedOnStartUp;
        [SerializeField] internal bool isAppeared;
        internal bool IsAppearing;
        internal bool IsDisappearing;

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
            isAppeared = isAppearedOnStartUp;
            if (isAppeared)
            {
                canvasGroup.alpha = 1f;
                isAppeared = true;
                canvasGroup.blocksRaycasts = true;
                canvasGroup.interactable = true;
            }
            else
            {
                canvasGroup.alpha = 0f;
                isAppeared = false;
                canvasGroup.blocksRaycasts = false;
                canvasGroup.interactable = false;
            }
        }

        public async void SwapAppearanceStatus()
        {
            if (isAppeared)
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
            IsAppearing = true;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;

            OnPreAppear();
            canvasGroup.alpha = 1f;
            // await canvasGroup.DOFade(1f, binderUI.GetCanvasAppearDuration()).AsyncWaitForCompletion();

            isAppeared = true;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
            IsAppearing = false;

            OnPostAppear();
        }

        public async UniTask Disappear()
        {
            if (!CanDisappear())
            {
                return;
            }

            IsDisappearing = true;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            OnPreDisappear();
            canvasGroup.alpha = 0f;
            // await canvasGroup.DOFade(0f, binderUI.GetCanvasDisappearDuration()).AsyncWaitForCompletion();
            isAppeared = false;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            IsDisappearing = false;
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

        public virtual void OnEscapeRequested()
        {
            Disappear().Forget();
        }

        public virtual void BeforeGameSessionInitialized()
        {
        }

        public virtual void BeforeGameSceneUnloaded()
        {
        }

        public virtual void AfterGameSessionInitialized()
        {
        }
    }
}