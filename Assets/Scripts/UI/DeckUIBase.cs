using Cysharp.Threading.Tasks;
using Deck.Data.UI;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Deck.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class DeckUIBase : MonoBehaviour
    {
        [Inject] protected DiContainer container;
        [Inject] protected DeckBinderUI BinderUI;
        [SerializeField] protected CanvasGroup canvasGroup;
        [SerializeField] protected bool isAppearedOnStartUp;
        [SerializeField] protected RectTransform rectTransform;

        [ShowInInspector, ReadOnly] internal bool _isAppeared;
        internal bool isAppearing;
        internal bool isDisappearing;

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
                Debug.LogError("Disappearing");
                await Disappear();
            }
            else
            {
                Debug.LogError("Appearing");
                await Appear();
            }
        }

        public async UniTask Appear()
        {
            isAppearing = true;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;

            OnPreAppear();
            await canvasGroup.DOFade(1f, BinderUI.GetCanvasAppearDuration()).AsyncWaitForCompletion();

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
            await canvasGroup.DOFade(0f, BinderUI.GetCanvasDisappearDuration()).AsyncWaitForCompletion();
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

        public virtual void OnPreAppear()
        {
        }

        public virtual void OnPostAppear()
        {
        }

        public virtual void OnPreDisappear()
        {
        }

        public virtual void OnPostDisappear()
        {
        }

        public virtual bool CanDisappear()
        {
            return true;
        }
    }
}